namespace CanvasBid.Models
{
    public class Bid
    {
        public int Id { get; set; }

        // Artwork reference
        public int ArtworkId { get; set; }
        public Artwork Artwork { get; set; } = null!;

        // Bidder reference
        public int BidderId { get; set; }
        public User Bidder { get; set; } = null!;

        // Bid details
        public decimal Amount { get; set; }
        public DateTime BiddingTime { get; set; } = DateTime.UtcNow;
        public bool IsOutbid { get; set; } = false;
    }
}