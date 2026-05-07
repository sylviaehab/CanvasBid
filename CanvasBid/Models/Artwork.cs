namespace CanvasBid.Models
{
    public enum ArtworkStatus
    {
        Active,
        Sold,
        Expired,
        Approved,
        Rejected,
        pending

    }

    public class Artwork
    {
        // Artist info
        public int Id { get; set; }
        public int ArtistID { get; set; }
        public User Artist { get; set; } = null!;

        // Artwork details
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public decimal StartingPrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Auction details
        public DateTime AuctionStartTime { get; set; }
        public DateTime AuctionEndTime { get; set; }

        // Status
        public ArtworkStatus Status { get; set; }

        // Winner details
        public int WinnerID { get; set; }
        public decimal finalPrice { get; set; }

        // Navigation properties
        public ICollection<Bid> Bids { get; set; } = new List<Bid>();
        public ICollection<Watchlist> WatchlistedBy { get; set; } = new List<Watchlist>();
    }
}
