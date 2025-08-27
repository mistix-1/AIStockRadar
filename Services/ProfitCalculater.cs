using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AIStockRadar.Models;

namespace AIStockRadar.Services
{
    /// <summary>
    /// Builds a cumulative realized P&L series from Trades.
    /// Uses Trade.Side ("Buy"/"Sell") with positive Quantity.
    /// </summary>
    public static class ProfitCalculator
    {
        public static void BuildProfitSeriesForUser(AppDbContext db, int userId,
            out List<string> labels, out List<decimal> values)
        {
            var trades = db.Trades
                .Include(t => t.Security)
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.TradeDate)
                .AsNoTracking()
                .ToList();

            labels = new List<string>();
            values = new List<decimal>();
            if (trades.Count == 0) return;

            // per-security average-cost state
            var pos = new Dictionary<int, (decimal qty, decimal costBasis)>();
            decimal cumulativeRealized = 0m;

            foreach (var t in trades)
            {
                if (!pos.TryGetValue(t.SecurityId, out var state))
                    state = (0m, 0m);

                if (t.IsBuy)
                {
                    // add shares at price
                    state.qty += t.Quantity;
                    state.costBasis += t.Quantity * t.Price;
                }
                else if (t.IsSell)
                {
                    var sellQty = t.Quantity; // always positive
                    var availableQty = state.qty;

                    // protect against accidental over-sell (shouldn't happen with your constraints, but just in case)
                    if (sellQty > availableQty && availableQty > 0)
                        sellQty = availableQty;

                    var avgCost = state.qty > 0 ? (state.costBasis / state.qty) : 0m;
                    var realized = (t.Price - avgCost) * sellQty;
                    cumulativeRealized += realized;

                    // reduce open position at avg cost
                    state.qty -= sellQty;
                    state.costBasis -= avgCost * sellQty;
                }

                pos[t.SecurityId] = state;

                labels.Add(t.TradeDate.ToString("yyyy-MM-dd HH:mm"));
                values.Add(decimal.Round(cumulativeRealized, 2));
            }
        }
    }
}
