using CanvasBid.DTOS.UserDTOS;

namespace CanvasBid.Services;

public interface IUserService
{
    Task<UserResponseDto?> GetUserByIdAsync(int id);
    Task<IEnumerable<UserResponseDto>> GetAllArtistsAsync();
    Task<IEnumerable<UserResponseDto>> GetAllBuyersAsync();
    Task<bool> UpdateUserAsync(int userId, UserResponseDto dto);
    Task<bool> DeactivateUserAsync(int userId);
}