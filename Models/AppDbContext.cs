using AIStockRadar.Models;
using Microsoft.EntityFrameworkCore;

namespace YourProjectName.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        // later you'll add: public DbSet<Holdings> Holdings { get; set; } etc.
    }
}
