namespace CanvasBid.DTOS.ArtworkDTOS
{


public class CreateArtworkDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    
    public decimal InitialPrice { get; set; }
    public decimal BuyNowPrice { get; set; }
    
    public DateTime AuctionStartTime { get; set; }
    public DateTime AuctionEndTime { get; set; }
}
}