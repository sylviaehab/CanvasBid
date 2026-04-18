namespace CanvasBid.Models;

public enum UserRole
{
    Admin,
    Artist,
    Buyer
}

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<Artwork> Artworks { get; set; } = new List<Artwork>();
    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public ICollection<Watchlist> Watchlists { get; set; } = new List<Watchlist>();
}
