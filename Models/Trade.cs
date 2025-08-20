using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AIStockRadar.Models
{
    [Index(nameof(UserId), nameof(SecurityId), nameof(TradeDate))] // helpful query index
    public class Trade
    {
        [Key] public int TradeId { get; set; }                         // PK

        [Required, ForeignKey(nameof(User))]
        public int UserId { get; set; }                                // FK → User

        [Required, ForeignKey(nameof(Security))]
        public int SecurityId { get; set; }                            // FK → Security

        public DateTime TradeDate { get; set; } = DateTime.UtcNow;

        // Always positive; Side tells buy/sell
        [Precision(18, 6)]
        [Range(typeof(decimal), "0.000001", "79228162514264337593543950335")]
        public decimal Quantity { get; set; }

        [Precision(18, 6)]
        [Range(typeof(decimal), "0.0", "79228162514264337593543950335")]
        public decimal Price { get; set; }

        [Required, MaxLength(4)]
        [RegularExpression("(?i)^(Buy|Sell)$", ErrorMessage = "Side must be 'Buy' or 'Sell'.")]
        public string Side { get; set; } = "Buy";

        // Navigations
        public User? User { get; set; }
        public Security? Security { get; set; }

        // Helpers (not stored)
        [NotMapped] public bool IsBuy => Side?.Equals("Buy", StringComparison.OrdinalIgnoreCase) == true;
        [NotMapped] public bool IsSell => Side?.Equals("Sell", StringComparison.OrdinalIgnoreCase) == true;
        [NotMapped] public decimal SignedQuantity => IsBuy ? Quantity : -Quantity;
        [NotMapped] public decimal SignedValue => SignedQuantity * Price;
    }
}
