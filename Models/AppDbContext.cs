using Microsoft.EntityFrameworkCore;

namespace AIStockRadar.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UsersInfo> UsersInfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationship: UsersInfo -> User (required)
            modelBuilder.Entity<UsersInfo>()
                .HasOne(ui => ui.User)
                .WithMany() // no collection on User
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure one UsersInfo per User
            modelBuilder.Entity<UsersInfo>()
                .HasIndex(ui => ui.UserId)
                .IsUnique();

            // Capital precision (SQLite will respect decimal mapping via EF)
            modelBuilder.Entity<UsersInfo>()
                .Property(ui => ui.Capital)
                .HasColumnType("decimal(18,2)");
        }
    }
}
