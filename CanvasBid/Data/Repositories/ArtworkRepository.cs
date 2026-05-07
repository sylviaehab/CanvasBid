using CanvasBid.Models;
namespace CanvasBid.Data.Repositories;
using CanvasBid.Data;
using Microsoft.EntityFrameworkCore;
public class ArtworkRepository : Repository<Artwork>, IArtworkRepository
{
    public ArtworkRepository(CanvasBidDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Artwork>> artworksByArtistIdAsync(int artistId)
    {
        return await _dbSet.Where(a=>a.ArtistID==artistId)
        .OrderByDescending(a=>a.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Artwork>> GetActiveAuctionsAsync()

    {
          var now = DateTime.UtcNow;
        return await _dbSet.Where(a=>a.Status==ArtworkStatus.Active && a.AuctionStartTime<=now && a.AuctionEndTime>now)
        .OrderByDescending(a=>a.AuctionEndTime).ToListAsync();
    }

    public async Task<IEnumerable<Artwork>> GetArtworksByCategoryAsync(string category)
    {
        return await _dbSet.Where(a=>a.Category==category && a.Status==ArtworkStatus.Approved)
        .OrderByDescending(a=>a.CreatedAt).ToListAsync();
    }

    public async Task<IEnumerable<Artwork>> GetArtworksByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet.Where(a=>a.StartingPrice>=minPrice && a.StartingPrice<=maxPrice && a.Status==ArtworkStatus.Approved)
        .OrderByDescending(a=>a.CreatedAt).ToListAsync();
    }

    public async Task<Artwork?> GetByIdWithDetailsAsync(int id)
    {
         return await _dbSet
            .Include(a => a.Artist)
            .Include(a => a.Bids)
            .Include(a => a.WatchlistedBy)
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<Artwork>> GetEndedAuctionsAsync()
    {
         var now = DateTime.UtcNow;
        return await _dbSet
            .Where(a => a.AuctionEndTime <= now)
            .OrderByDescending(a => a.AuctionEndTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Artwork>> GetPendingApprovalAsync()
    {
        return await _dbSet.Where(a => a.Status == ArtworkStatus.pending)
        .OrderBy(a => a.CreatedAt)
        .ToListAsync();
    }

    public async Task<IEnumerable<Artwork>> GetWithBidsAsync(int artworkId)
    {
      return await _dbSet.Include(a=>a.Id==artworkId).Include(a=>a.Bids).ToListAsync();
    }

    public async Task<IEnumerable<Artwork>> SearchByCategoryAsync(string category)
    {
       return await _dbSet
            .Where(a => a.Category.Contains(category) && a.Status == ArtworkStatus.Approved)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Artwork>> SearchByTitleAsync(string title)
    {
        return await _dbSet
            .Where(a => a.Title.Contains(title) && a.Status == ArtworkStatus.Approved)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }
}