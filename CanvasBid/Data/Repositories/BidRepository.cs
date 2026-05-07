using CanvasBid.Models;
namespace CanvasBid.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using CanvasBid.Data;
public class BidRepository : Repository<Bid>, IBidRepository
{
    private const decimal MINIMUM_BID_INCREMENT = 10m;

    public BidRepository(CanvasBidDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Bid>> GetByArtworkAsync(int artworkId)
    {
        return await _dbSet
            .Where(b => b.ArtworkId == artworkId)
            .OrderByDescending(b => b.BiddingTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Bid>> GetByBidderAsync(int bidderId)
    {
        return await _dbSet
            .Where(b => b.BidderId == bidderId)
            .OrderByDescending(b => b.BiddingTime)
            .Include(b => b.Artwork)
            .ToListAsync();
    }

    public async Task<Bid?> GetHighestBidAsync(int artworkId)
    {
        return await _dbSet
            .Where(b => b.ArtworkId == artworkId)
            .OrderByDescending(b => b.Amount)
            .FirstOrDefaultAsync();
    }

    public async Task<int> GetBidCountAsync(int artworkId)
    {
        return await _dbSet
            .Where(b => b.ArtworkId == artworkId)
            .CountAsync();
    }

    public async Task<decimal> GetHighestBidAmountAsync(int artworkId)
    {
        var highestBid = await GetHighestBidAsync(artworkId);
        return highestBid?.Amount ?? 0;
    }

    public async Task<IEnumerable<Bid>> GetBidHistoryAsync(int artworkId)
    {
        return await _dbSet
            .Where(b => b.ArtworkId == artworkId)
            .Include(b => b.Bidder)
            .OrderByDescending(b => b.BiddingTime)
            .ToListAsync();
    }

    public async Task<bool> BidExceedsMinimumAsync(int artworkId, decimal newBidAmount)
    {
        var artwork = await _context.Artworks.FindAsync(artworkId);
        if (artwork == null)
            return false;

        var highestBid = await GetHighestBidAmountAsync(artworkId);
        if (highestBid == 0)
        {
            return newBidAmount >= artwork.StartingPrice;
        }

        return newBidAmount >= highestBid + MINIMUM_BID_INCREMENT;
        

    }
}