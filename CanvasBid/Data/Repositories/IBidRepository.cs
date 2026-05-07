using CanvasBid.Models;
namespace CanvasBid.Data.Repositories;
public interface IBidRepository : IRepository<Bid>
{
    Task<IEnumerable<Bid>> GetByArtworkAsync(int artworkId);
    Task<IEnumerable<Bid>> GetByBidderAsync(int bidderId);
    Task<Bid> GetHighestBidAsync(int artworkId);
    Task<int> GetBidCountAsync(int artworkId);
    Task<decimal> GetHighestBidAmountAsync(int artworkId);
    Task<IEnumerable<Bid>> GetBidHistoryAsync(int artworkId);
    Task<bool> BidExceedsMinimumAsync(int artworkId, decimal amount);
}