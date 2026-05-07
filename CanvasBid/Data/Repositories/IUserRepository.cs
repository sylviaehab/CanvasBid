namespace CanvasBid.Data.Repositories;
using CanvasBid.Models;
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
   Task<IEnumerable<User>> GetArtistsAsync();
    Task<IEnumerable<User>> GetBuyersAsync();
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
}

