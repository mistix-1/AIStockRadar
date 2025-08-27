using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AIStockRadar.Models;
using AIStockRadar.ViewModels;
using AIStockRadar.Services;

public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    public DashboardController(AppDbContext db) => _db = db;

    [HttpGet("/Dashboard")]
    public IActionResult Dashboard()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToAction("SignIn", "Account");

        // Left: holdings with Quantity > 0 only
        var holdings = _db.UserHoldings
            .Include(h => h.Security) // so we can show ticker
            .Where(h => h.UserId == userId.Value && h.Quantity > 0m)
            .OrderBy(h => h.Security!.Ticker)
            .AsNoTracking()
            .ToList();

        // Right: realized P&L over time (from Trades)
        ProfitCalculator.BuildProfitSeriesForUser(_db, userId.Value,
            out var labels, out var values);

        var vm = new DashboardViewModel
        {
            Holdings = holdings,
            ProfitLabels = labels,
            ProfitValues = values
        };

        return View(vm);
    }
}
