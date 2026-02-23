using Microsoft.AspNetCore.Mvc;
using AIStockRadar.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

public class AIController : Controller
{
    private readonly AppDbContext _context;

    // This connects the controller to your database
    public AIController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("/AIRadar")]
    public async Task<IActionResult> Index()
    {
        // 1. Check if user is logged in
        var userId = HttpContext.Session.GetInt32("UserId");
        if (!userId.HasValue)
        {
            return RedirectToAction("SignIn", "Account");
        }

        // 2. Get the RiskTolerance ("Bad", "Ok", "Good") from your Users.db
        var info = _context.UsersInfos.FirstOrDefault(u => u.UserId == userId.Value);
        string dbRisk = info?.RiskTolerance ?? "Ok";

        // 3. Map it to what your Python code expects ("Low Risk", "Mid Risk", "High Risk")
        string aiRisk = dbRisk switch
        {
            "Good" => "High Risk",
            "Bad" => "Low Risk",
            _ => "Mid Risk"
        };

        // 4. Call the Colab Link (PASTE YOUR NEW LINK HERE EVERY TIME)
        // Important: Use /api/recommend at the end if that's what your Python code defines
        string colabUrl = $"https://YOUR-LINK.ngrok-free.app/api/recommend?risk={aiRisk}";

        var viewModel = new AiRecommendationViewModel();

        using (var client = new HttpClient())
        {
            try
            {
                // Set a timeout so the website doesn't hang forever if Colab is slow
                client.Timeout = TimeSpan.FromSeconds(20);

                var response = await client.GetStringAsync(colabUrl);

                // Convert the JSON text from Python into a C# object
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                viewModel = JsonSerializer.Deserialize<AiRecommendationViewModel>(response, options);
            }
            catch (Exception ex)
            {
                viewModel.ErrorMessage = "The AI Model is currently offline. Please ensure the Colab notebook is running.";
            }
        }

        // Send the data to your AIRadar.cshtml page
        return View("AIRadar", viewModel);
    }
}