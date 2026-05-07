using CanvasBid.Data;
using CanvasBid.Models;
using Microsoft.EntityFrameworkCore;    
namespace CanvasBid.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
        
    public UserRepository(CanvasBidDbContext context) : base(context)
    {
    }
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbSet.AnyAsync(u=>u.Email==email);
    }

    public async Task<IEnumerable<User>> GetArtistsAsync()
    {
        return await _dbSet.Where(u => u.Role ==UserRole.Artist).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetBuyersAsync()
    {
     return await _dbSet.Where(u => u.Role ==UserRole.Buyer).ToListAsync();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
       return await _dbSet.FirstOrDefaultAsync(u=>u.Email==email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _dbSet.AnyAsync(u=>u.UserName==username);
    }
}