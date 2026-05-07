using Microsoft.EntityFrameworkCore;
using CanvasBid.Data;
namespace CanvasBid.Data.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly CanvasBidDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(CanvasBidDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate)
    {
        return await Task.FromResult(_dbSet.Where(predicate).AsEnumerable());
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

     public void DeleteRange(IEnumerable<T> entities)
     {
         _dbSet.RemoveRange(entities);
     }

     public async Task SaveChangesAsync()
     {
         await _context.SaveChangesAsync();
     }

   
}