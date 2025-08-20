using Microsoft.EntityFrameworkCore;

namespace AIStockRadar.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UsersInfo> UsersInfos { get; set; }

        public DbSet<Security> Securities { get; set; }
        public DbSet<Trade> Trades { get; set; }
        public DbSet<UserHolding> UserHoldings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- UsersInfo (existing) ----
            modelBuilder.Entity<UsersInfo>()
                .HasOne(ui => ui.User)
                .WithMany()
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UsersInfo>()
                .HasIndex(ui => ui.UserId)
                .IsUnique();

            modelBuilder.Entity<UsersInfo>()
                .Property(ui => ui.Capital)
                .HasColumnType("decimal(18,2)");

            // ---- Securities ----
            modelBuilder.Entity<Security>(e =>
            {
                e.HasKey(s => s.SecurityId);
                e.Property(s => s.Ticker).IsRequired().HasMaxLength(16);
                e.HasIndex(s => s.Ticker).IsUnique();               // 1 row per ticker
            });

            // ---- Trades ----
            modelBuilder.Entity<Trade>(e =>
            {
                e.HasKey(t => t.TradeId);
                e.Property(t => t.Quantity).HasPrecision(18, 6);
                e.Property(t => t.Price).HasPrecision(18, 6);
                e.Property(t => t.Side).HasMaxLength(4).IsRequired();

                e.HasOne(t => t.User)
                    .WithMany()
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(t => t.Security)
                    .WithMany()
                    .HasForeignKey(t => t.SecurityId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasIndex(t => new { t.UserId, t.SecurityId, t.TradeDate });

                // Basic data quality
                e.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_Trade_Quantity_Positive", "Quantity > 0");
                    tb.HasCheckConstraint("CK_Trade_Price_NonNegative", "Price >= 0");
                });
            });

            // ---- UserHoldings ----
            modelBuilder.Entity<UserHolding>(e =>
            {
                e.HasKey(h => h.HoldingId);
                e.Property(h => h.Quantity).HasPrecision(18, 6);
                e.Property(h => h.AvgCost).HasPrecision(18, 6);

                e.HasOne(h => h.User)
                    .WithMany()
                    .HasForeignKey(h => h.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(h => h.Security)
                    .WithMany()
                    .HasForeignKey(h => h.SecurityId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Prevent duplicate rows for the same (User, Security)
                e.HasIndex(h => new { h.UserId, h.SecurityId }).IsUnique();

                // No negative positions
                e.ToTable(tb =>
                {
                    tb.HasCheckConstraint("CK_UserHolding_Quantity_NonNegative", "Quantity >= 0");
                });
            });
        }
    }
}
