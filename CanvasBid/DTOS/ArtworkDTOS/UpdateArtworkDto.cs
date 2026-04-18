namespace CanvasBid.DTOS.ArtworkDTOS
{
public class UpdateArtworkDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public string? ImageUrl { get; set; }
    
    public decimal? InitialPrice { get; set; }
    public decimal? BuyNowPrice { get; set; }
    
    public DateTime? AuctionStartTime { get; set; }
    public DateTime? AuctionEndTime { get; set; }
    
    public string? Status { get; set; } // "Approved", "Rejected", "Cancelled"
}}