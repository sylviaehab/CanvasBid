using CanvasBid.Models;
using Microsoft.EntityFrameworkCore;

namespace CanvasBid.Data;

public class CanvasBidDbContext : DbContext
{
	public CanvasBidDbContext(DbContextOptions<CanvasBidDbContext> options) : base(options)
	{
	}

	public DbSet<User> Users => Set<User>();
	public DbSet<Artwork> Artworks => Set<Artwork>();
	public DbSet<Bid> Bids => Set<Bid>();
	public DbSet<Watchlist> Watchlists => Set<Watchlist>();

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<User>(entity =>
		{
			entity.HasKey(u => u.Id);
			entity.Property(u => u.UserName).IsRequired().HasMaxLength(100);
			entity.Property(u => u.Email).IsRequired().HasMaxLength(200);
			entity.Property(u => u.Password).IsRequired().HasMaxLength(255);
		});

		modelBuilder.Entity<Artwork>(entity =>
		{
			entity.HasKey(a => a.Id);
			entity.Property(a => a.Title).IsRequired().HasMaxLength(200);
			entity.Property(a => a.Category).HasMaxLength(100);
			entity.Property(a => a.Tags).HasMaxLength(500);
			entity.Property(a => a.ImageUrl).HasMaxLength(1000);
			entity.Property(a => a.StartingPrice).HasPrecision(18, 2);
			entity.Property(a => a.finalPrice).HasPrecision(18, 2);

			entity.HasOne(a => a.Artist)
				.WithMany(u => u.Artworks)
				.HasForeignKey(a => a.ArtistID)
				.OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Bid>(entity =>
		{
			entity.HasKey(b => b.Id);
			entity.Property(b => b.Amount).HasPrecision(18, 2);

			entity.HasOne(b => b.Artwork)
				.WithMany(a => a.Bids)
				.HasForeignKey(b => b.ArtworkId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(b => b.Bidder)
				.WithMany(u => u.Bids)
				.HasForeignKey(b => b.BidderId)
				.OnDelete(DeleteBehavior.Restrict);
		});

		modelBuilder.Entity<Watchlist>(entity =>
		{
			entity.HasKey(w => w.Id);
			entity.HasIndex(w => new { w.UserId, w.ArtworkId }).IsUnique();

			entity.HasOne(w => w.user)
				.WithMany(u => u.Watchlists)
				.HasForeignKey(w => w.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(w => w.artwork)
				.WithMany(a => a.WatchlistedBy)
				.HasForeignKey(w => w.ArtworkId)
				.OnDelete(DeleteBehavior.Cascade);
		});
	}
}
