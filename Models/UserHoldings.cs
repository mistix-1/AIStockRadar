using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AIStockRadar.Models
{
    [Index(nameof(UserId), nameof(SecurityId), IsUnique = true)]   // one holding per (user, security)
    public class UserHolding
    {
        [Key] public int HoldingId { get; set; }                   // PK

        [Required, ForeignKey(nameof(User))]
        public int UserId { get; set; }                            // FK → User

        [Required, ForeignKey(nameof(Security))]
        public int SecurityId { get; set; }                        // FK → Security

        [Precision(18, 6)]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Quantity { get; set; }                      // current open shares

        [Precision(18, 6)]
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal AvgCost { get; set; }                       // weighted avg cost of open position

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigations
        public User? User { get; set; }
        public Security? Security { get; set; }
    }
}
