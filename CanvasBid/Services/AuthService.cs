using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CanvasBid.DTOS.UserDTOS;
using CanvasBid.Models;
using CanvasBid.Data.Repositories;
using AutoMapper;
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

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(20);

            var hashWithSalt = new byte[36];
            Array.Copy(salt, 0, hashWithSalt, 0, 16);
            Array.Copy(hash, 0, hashWithSalt, 16, 20);

            return Convert.ToBase64String(hashWithSalt);
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(hash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash2 = pbkdf2.GetBytes(20);

            for (int i = 0; i < 20; i++)
            {
                if (hashBytes[i + 16] != hash2[i])
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
    public async Task<(bool success, string Message, string? token, UserResponseDto user)> LoginAsync(LoginUserDto loginUserDto)
    {
        try
        {
            if(string.IsNullOrEmpty(loginUserDto.Email)|| string.IsNullOrEmpty(loginUserDto.Password))
            {
                return (false, "Email and password are required", null, null);
            }
            var user=await _userRepository.GetByEmailAsync(loginUserDto.Email);
            if(user==null || user.Password != loginUserDto.Password)
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
            if(string.IsNullOrEmpty(registerUserDto.Username)||string.IsNullOrEmpty(registerUserDto.Email)|| string.IsNullOrEmpty(registerUserDto.Password))
            {
                return (false, "All fields are required", null);
            }
            if (registerUserDto.Password != registerUserDto.ConfirmPassword)
            {
                return (false, "Passwords do not match", null);
            }
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
                Password=registerUserDto.Password
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
}