namespace CanvasBid.Services;
using CanvasBid.DTOS.UserDTOS;
using Microsoft.EntityFrameworkCore;
using CanvasBid.Data;
using AutoMapper;
using CanvasBid.Data.Repositories;

public class UserService : IUserService

{
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    public UserService(IMapper mapper, IUserRepository userRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
    }
    public async Task<bool> DeactivateUserAsync(int userId)
    {
        try
        {
            var user=await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            user.IsActive=false;
            user.UpdatedAt=DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            return true;

        }
        catch (Exception ex)
        {
            
            throw new Exception($"Error deactivating user: {ex.Message}");
        }
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllArtistsAsync()
    {
        try
        {
            var artist= await _userRepository.GetArtistsAsync();
            return _mapper.Map< IEnumerable<UserResponseDto>>(artist);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting artists: {ex.Message}");
        }
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllBuyersAsync()
    {
         try
        {
            var buyer= await _userRepository.GetBuyersAsync();
            return _mapper.Map< IEnumerable<UserResponseDto>>(buyer);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting buyers: {ex.Message}");
        }
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(int id)
    {
          try
        {
            var user= await _userRepository.GetByIdAsync(id);
            return _mapper.Map< UserResponseDto>(user);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error getting user: {ex.Message}");
        }
    }

    public async Task<bool> UpdateUserAsync(int userId, UserResponseDto dto)
    {
        try
        {
            var user= await _userRepository.GetByIdAsync(userId);
            if (user == null) {
                return false;
            }
            user.UpdatedAt=DateTime.UtcNow;
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error updating user: {ex.Message}");
        }
    }
}