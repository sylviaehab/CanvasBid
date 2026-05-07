using CanvasBid.DTOS.UserDTOS;
namespace CanvasBid.Services;
public interface IAuthServices
{
    Task<(bool success, string Message,UserResponseDto? user)> RegisterAsync(RegisterUserDto registerUserDto);
    Task<(bool success, string Message, string? token, UserResponseDto user)> LoginAsync(LoginUserDto loginUserDto);
    string GenerateJwtToken(UserResponseDto user);
}