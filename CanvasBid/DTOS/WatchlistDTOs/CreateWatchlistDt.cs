namespace CanvasBid.DTOS.WatchlistDTOs
{
    public class CreateWatchlistDto
{
    public int ArtworkId { get; set; }
    public bool NotifyOnNewBid { get; set; } = true;
    public bool NotifyOnAuctionEnding { get; set; } = true;
}
}