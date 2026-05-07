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
    public string GenerateJwtToken(UserResponseDto user)
    {
        throw new NotImplementedException();
    }

    public Task<(bool success, string Message, string? token, UserResponseDto user)> LoginAsync(LoginUserDto loginUserDto)
    {
        throw new NotImplementedException();
    }

    public Task<(bool success, string Message, UserResponseDto? user)> RegisterAsync(RegisterUserDto registerUserDto)
    {
        throw new NotImplementedException();
    }
}