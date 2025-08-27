using Microsoft.AspNetCore.Mvc;

public class AIController : Controller
{
    [HttpGet("/AIRadar")]
    public IActionResult Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("SignIn", "Account");
        }

        return View("AIRadar"); // explicitly return AIRadar.cshtml
    }
}
