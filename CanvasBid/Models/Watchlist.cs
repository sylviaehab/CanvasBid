namespace CanvasBid.Models
{
    public class Watchlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User user { get; set; } = null!;

        public int ArtworkId { get; set; }
        public Artwork artwork { get; set; } = null!;
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        // Notification Preferences
        public bool NotifyOnNewBid { get; set; } = true;      // Notify when someone bids
        public bool NotifyOnAuctionEnding { get; set; } = true; // Notify when auction ends
    }
}