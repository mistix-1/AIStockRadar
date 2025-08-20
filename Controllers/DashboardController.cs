using Microsoft.AspNetCore.Mvc;

public class DashboardController : Controller
{
    [HttpGet("/Dashboard")]
    public IActionResult Dashboard()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue) return RedirectToAction("SignIn", "Account");

        return View();
    }
}
