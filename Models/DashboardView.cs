using System.Collections.Generic;
using AIStockRadar.Models;

namespace AIStockRadar.ViewModels
{
    public class DashboardViewModel
    {
        public List<UserHolding> Holdings { get; set; } = new();
        public List<string> ProfitLabels { get; set; } = new();   // "yyyy-MM-dd HH:mm"
        public List<decimal> ProfitValues { get; set; } = new();  // cumulative realized P&L
    }
}
