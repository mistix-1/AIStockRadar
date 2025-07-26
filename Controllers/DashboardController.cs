using Microsoft.AspNetCore.Mvc;

namespace AIStockRadar.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
