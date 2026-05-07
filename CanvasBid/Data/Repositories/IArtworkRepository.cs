using CanvasBid.Models;
namespace CanvasBid.Data.Repositories;
public interface IArtworkRepository : IRepository<Artwork>
{
    Task<IEnumerable<Artwork>> artworksByArtistIdAsync(int artistId);
     Task<IEnumerable<Artwork>> GetArtworksByCategoryAsync(string category);
   Task <IEnumerable<Artwork>> GetActiveAuctionsAsync();
    Task<IEnumerable<Artwork>> GetArtworksByPriceRangeAsync(decimal minPrice, decimal maxPrice);
     Task<IEnumerable<Artwork>> SearchByTitleAsync(String title);
     






}