using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AIStockRadar.Models
{
    [Index(nameof(Ticker), IsUnique = true)]               // one row per ticker
    public class Security
    {
        [Key] public int SecurityId { get; set; }          // PK

        [Required, MaxLength(16)]
        public string Ticker { get; set; } = string.Empty;  // store uppercase
    }
}
