using AIStockRadar.Models;
using Microsoft.EntityFrameworkCore;
using AIStockRadar.Models;

namespace AIStockRadar.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
