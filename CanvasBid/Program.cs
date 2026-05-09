using CanvasBid.Data;
using CanvasBid.DTOS.UserDTOS;
using CanvasBid.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CanvasBid.Services;
using CanvasBid.Mappings;

var builder = WebApplication.CreateBuilder(args);

// ===== CONFIGURATION =====
var configuration = builder.Configuration;
var jwtSettings = configuration.GetSection("Jwt");
var jwtSecret = jwtSettings["Secret"];
var jwtIssuer = jwtSettings["Issuer"];
var jwtAudience = jwtSettings["Audience"];

// ===== DATABASE CONFIGURATION =====
builder.Services.AddDbContext<CanvasBidDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.CommandTimeout(30)
    )
);

// ===== REPOSITORY REGISTRATION =====
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IArtworkRepository, ArtworkRepository>();
builder.Services.AddScoped<IBidRepository, BidRepository>();

// ===== SERVICE REGISTRATION =====
builder.Services.AddScoped<IAuthServices, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();


// ===== AUTOMAPPER CONFIGURATION =====
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// ===== JWT AUTHENTICATION CONFIGURATION =====
if (string.IsNullOrEmpty(jwtSecret))
{
    Console.WriteLine("⚠️ JWT Secret not configured; skipping authentication setup.");
}
else
{
    var key = Encoding.ASCII.GetBytes(jwtSecret);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // For SignalR with JWT
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/biddingHub"))
                {
                    context.Token = accessToken;
                }
                
                return Task.CompletedTask;
            }
        };
    });

    // ===== AUTHORIZATION CONFIGURATION =====
    builder.Services.AddAuthorization();
}

// ===== CORS CONFIGURATION =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });

    // Or more restrictive CORS for production:
    // options.AddPolicy("AllowSpecific", policy =>
    // {
    //     policy
    //         .WithOrigins("https://yourdomain.com")
    //         .AllowAnyMethod()
    //         .AllowAnyHeader()
    //         .AllowCredentials();
    // });
});

// ===== SIGNALR CONFIGURATION =====
builder.Services.AddSignalR();

// ===== CONTROLLERS & SWAGGER =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "CanvasBid API",
        Version = "v1",
        Description = "Online Art Auction Platform API with Real-time Bidding",
        Contact = new Microsoft.OpenApi.OpenApiContact
        {
            Name = "CanvasBid Support",
            Email = "support@canvasbid.com"
        }
    });

    // Add JWT Bearer to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Enter your JWT as: Bearer {token}"
    });

    options.AddSecurityRequirement(_ => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", null, null),
            new List<string>()
        }
    });

    // Add XML comments for documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// ===== BUILD APPLICATION =====
var app = builder.Build();

// ===== DATABASE INITIALIZATION =====
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CanvasBidDbContext>();
    
    try
    {
        // Create database if it doesn't exist
        dbContext.Database.EnsureCreated();
        
       
        
        Console.WriteLine("✅ Database initialized successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Database initialization failed: {ex.Message}");
    }
}

// ===== MIDDLEWARE PIPELINE =====

// Swagger documentation (Development only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CanvasBid API v1");
        options.RoutePrefix = string.Empty; // Make Swagger the default page
    });
}

// HTTPS redirection
app.UseHttpsRedirection();

// Routing
app.UseRouting();

// CORS
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapControllers();
//app.MapHub<BiddingHub>("/biddingHub");

// ===== ERROR HANDLING =====
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        
        var exceptionHandlerPathFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
        
        await context.Response.WriteAsJsonAsync(new
        {
            error = "An unexpected error occurred",
            message = app.Environment.IsDevelopment() ? exceptionHandlerPathFeature?.Error.Message : "Internal server error",
            timestamp = DateTime.UtcNow
        });
    });
});

// ===== RUN APPLICATION =====
Console.WriteLine("🚀 Starting CanvasBid API...");
app.Run();