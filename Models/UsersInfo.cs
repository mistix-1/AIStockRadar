using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AIStockRadar.Models
{
    public class UsersInfo
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required] public int Age { get; set; }
        [Required] public decimal Capital { get; set; }          // ₪
        [Required] public string RiskTolerance { get; set; }      // "Bad" | "Ok" | "Good"
        [Required] public string Priority1 { get; set; }          // "Buzz" | "Financials" | "GlobalTrend"
        [Required] public string Priority2 { get; set; }
        [Required] public string Priority3 { get; set; }
    }
}
