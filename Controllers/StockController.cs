using System;
using System.Threading.Tasks;
using AIStockRadar.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIStockRadar.Controllers
{
    public class StockController : Controller
    {
        private readonly AppDbContext _db;
        public StockController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult AddStock()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("SignIn", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStock(string side, string ticker, decimal price, decimal amount, string next = "add")
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue) return RedirectToAction("SignIn", "Account");

            var normalizedTicker = (ticker ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(side) ||
                string.IsNullOrWhiteSpace(normalizedTicker) ||
                price <= 0 || amount <= 0)
            {
                TempData["Error"] = "Please fill all fields with valid values.";
                return RedirectToAction("AddStock");
            }

            var isBuy = side.Equals("Buy", StringComparison.OrdinalIgnoreCase);

            await using var tx = await _db.Database.BeginTransactionAsync();

            // 1) Upsert Security
            var security = await _db.Securities.SingleOrDefaultAsync(s => s.Ticker == normalizedTicker);
            if (security == null)
            {
                security = new Security { Ticker = normalizedTicker };
                _db.Securities.Add(security);
                await _db.SaveChangesAsync();
            }

            // 2) Current holding (for sells)
            var holding = await _db.UserHoldings.SingleOrDefaultAsync(
                h => h.UserId == userId.Value && h.SecurityId == security.SecurityId);

            if (!isBuy)
            {
                var have = holding?.Quantity ?? 0m;
                if (have < amount)
                {
                    await tx.RollbackAsync();
                    TempData["Error"] = $"Not enough shares to sell. You have {have:0.######} {normalizedTicker}, tried to sell {amount:0.######}.";
                    return RedirectToAction("AddStock"); // show error on form
                }
            }

            // 3) Insert Trade (qty positive; Side = Buy/Sell)
            var trade = new Trade
            {
                UserId = userId.Value,
                SecurityId = security.SecurityId,
                TradeDate = DateTime.UtcNow,
                Side = isBuy ? "Buy" : "Sell",
                Quantity = amount,
                Price = price
            };
            _db.Trades.Add(trade);

            // 4) Update holdings
            if (holding == null)
            {
                if (!isBuy)
                {
                    await tx.RollbackAsync();
                    TempData["Error"] = $"Not enough shares to sell. You have 0 {normalizedTicker}.";
                    return RedirectToAction("AddStock");
                }

                holding = new UserHolding
                {
                    UserId = userId.Value,
                    SecurityId = security.SecurityId,
                    Quantity = amount,
                    AvgCost = price,
                    UpdatedAt = DateTime.UtcNow
                };
                _db.UserHoldings.Add(holding);
            }
            else
            {
                if (isBuy)
                {
                    var oldQty = holding.Quantity;
                    var newQty = oldQty + amount;
                    var newAvg = (oldQty * holding.AvgCost + amount * price) / (newQty == 0 ? 1 : newQty);
                    holding.Quantity = newQty;
                    holding.AvgCost = newAvg;
                }
                else
                {
                    var newQty = holding.Quantity - amount;
                    holding.Quantity = newQty;
                    if (newQty == 0) holding.AvgCost = 0;
                }

                holding.UpdatedAt = DateTime.UtcNow;
                _db.UserHoldings.Update(holding);
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            TempData["Success"] = $"{(isBuy ? "Bought" : "Sold")} {amount:0.######} {normalizedTicker} @ {price:0.######}.";

            // Route based on button clicked
            return next?.Equals("dashboard", StringComparison.OrdinalIgnoreCase) == true
                ? RedirectToAction("Dashboard", "Dashboard")
                : RedirectToAction("AddStock");
        }
    }
}
