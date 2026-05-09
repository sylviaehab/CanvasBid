using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CanvasBid.DTOS.UserDTOS;
using CanvasBid.Models;
using CanvasBid.Data.Repositories;
using AutoMapper;
using System.Security.Claims;
namespace CanvasBid.Services;

public class AuthService : IAuthServices
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
public AuthService(IUserRepository userRepository,IMapper mapper,IConfiguration configuration)
{    _userRepository=userRepository;
    _mapper=mapper;
    _configuration=configuration;}
       public string GenerateJwtToken(UserResponseDto user)
    {
        var jwtSecret = _configuration["Jwt:Secret"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new System.Security.Claims.Claim("sub", user.Id.ToString()),
            new System.Security.Claims.Claim("username", user.Username),
            new System.Security.Claims.Claim("email", user.Email),
            new System.Security.Claims.Claim("role", user.Role)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    
    
    public async Task<(bool success, string Message, string? token, UserResponseDto user)> LoginAsync(LoginUserDto loginUserDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(loginUserDto.Email) || string.IsNullOrWhiteSpace(loginUserDto.Password))
            {
                return (false, "Email and password are required", null, null);
            }
            var email = loginUserDto.Email.Trim();
            var user=await _userRepository.GetByEmailAsync(email);
            if(user==null || (!VerifyPassword(loginUserDto.Password, user.Password)))
            {
                return (false, "Invalid email or password", null, null);
            }
            if (!user.IsActive)
            {
                return (false,"User account id inactive", null, null);
            }
             var userResponse = _mapper.Map<UserResponseDto>(user);
            var token = GenerateJwtToken(userResponse);
            return (true, "Login successful", token, userResponse);
        }
        catch(Exception ex)
        {
            return (false, $"Login failed: {ex.Message}", null, null);
        }
        
    }

    public async Task<(bool success, string Message, UserResponseDto? user)> RegisterAsync(RegisterUserDto registerUserDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(registerUserDto.Username) || string.IsNullOrWhiteSpace(registerUserDto.Email) || string.IsNullOrWhiteSpace(registerUserDto.Password))
            {
                return (false, "All fields are required", null);
            }
            if (registerUserDto.Password != registerUserDto.ConfirmPassword)
            {
                return (false, "Passwords do not match", null);
            }
            registerUserDto.Email = registerUserDto.Email.Trim();
            if(await _userRepository.UsernameExistsAsync(registerUserDto.Username))
            {
                return (false, "Username already exists", null);
            }
            if(await _userRepository.EmailExistsAsync(registerUserDto.Email))
            {
                return (false, "Email already exists", null);
            }
            var user=new User
            {
                UserName=registerUserDto.Username,
                Email=registerUserDto.Email,
                IsActive=true,
                CreatedAt=DateTime.UtcNow,
                Role=registerUserDto.Role switch
                {
                    "Admin"=>UserRole.Admin,
                    "Artist"=>UserRole.Artist,
                    "Buyer"=>UserRole.Buyer,
                    _=>UserRole.Buyer
                },
                Password=HashPassword(registerUserDto.Password)
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            var userResponse=_mapper.Map<UserResponseDto>(user);
            return (true, "User registered successfully", userResponse);

        }
        catch(Exception ex)
        {
            return (false, $"Registration failed: {ex.Message}", null);
        }
    }
    private string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 20);

        var hashWithSalt = new byte[36];
        Buffer.BlockCopy(salt, 0, hashWithSalt, 0, 16);
        Buffer.BlockCopy(hash, 0, hashWithSalt, 16, 20);

        return Convert.ToBase64String(hashWithSalt);
    }

    private bool VerifyPassword(string password, string hash)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(hash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var hash2 = Rfc2898DeriveBytes.Pbkdf2(password, salt, 10000, HashAlgorithmName.SHA256, 20);

            var storedHash = new byte[20];
            Array.Copy(hashBytes, 16, storedHash, 0, 20);

            return CryptographicOperations.FixedTimeEquals(storedHash, hash2);
        }
        catch
        {
            return false;
        }
    }
}