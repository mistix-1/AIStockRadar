using AIStockRadar.Models; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace AIStockRadar.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        // Inject the database context
        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // Preferences (GET)
        // =========================
        [HttpGet]
        public IActionResult Prefrences()
        {
            // Prefill if user already has preferences
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId.HasValue)
            {
                var existing = _context.UsersInfos.FirstOrDefault(x => x.UserId == userId.Value);
                if (existing != null)
                {
                    var vm = new PreferencesViewModel
                    {
                        Age = existing.Age,
                        Capital = existing.Capital,
                        RiskTolerance = existing.RiskTolerance,
                        Priority1 = existing.Priority1,
                        Priority2 = existing.Priority2,
                        Priority3 = existing.Priority3
                    };
                    return View(vm); // Views/Account/Prefrences.cshtml
                }
            }

            return View(); // empty form
        }

        // =========================
        // Preferences (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Prefrences(PreferencesViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Simple validation: priorities must be all different
            if (model.Priority1 == model.Priority2 || model.Priority1 == model.Priority3 || model.Priority2 == model.Priority3)
            {
                ModelState.AddModelError("", "Priorities 1, 2, and 3 must be different.");
                return View(model);
            }

            // We’re relying on Session until proper auth/claims are added
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
            {
                // If no session user, send to SignIn
                return RedirectToAction("SignIn", "Account");
            }

            var existing = _context.UsersInfos.FirstOrDefault(x => x.UserId == userId.Value);
            if (existing == null)
            {
                var prefs = new UsersInfo
                {
                    UserId = userId.Value,
                    Age = model.Age,
                    Capital = model.Capital,
                    RiskTolerance = model.RiskTolerance,
                    Priority1 = model.Priority1,
                    Priority2 = model.Priority2,
                    Priority3 = model.Priority3
                };
                _context.UsersInfos.Add(prefs);
            }
            else
            {
                existing.Age = model.Age;
                existing.Capital = model.Capital;
                existing.RiskTolerance = model.RiskTolerance;
                existing.Priority1 = model.Priority1;
                existing.Priority2 = model.Priority2;
                existing.Priority3 = model.Priority3;
                _context.UsersInfos.Update(existing);
            }

            _context.SaveChanges();

            // After saving prefs, continue to Dashboard
            return RedirectToAction("Dashboard", "Dashboard");
        }

        // =========================
        // Sign Up
        // =========================
        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignUp(SignUpViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Passwords do not match.");
                return View(model);
            }

            
            var existingUser = _context.Users.FirstOrDefault(u => u.Email.ToLower() == model.Email.ToLower());
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(model);
            }

            
            var user = new User
            {
                Email = model.Email,
                PasswordHash = model.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // Store userId in Session for the Preferences POST
            HttpContext.Session.SetInt32("UserId", user.Id);

            // Send new users straight to preferences
            return RedirectToAction("Prefrences", "Account");
        }

        // =========================
        // Sign In
        // =========================
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SignIn(SignInViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == model.Email.ToLower());

            if (user == null || user.PasswordHash != model.Password)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

           
            HttpContext.Session.SetInt32("UserId", user.Id);

            
            bool hasPrefs = _context.UsersInfos.Any(ui => ui.UserId == user.Id);
            if (!hasPrefs)
            {
                return RedirectToAction("Prefrences", "Account");
            }

            // Otherwise proceed to Dashboard
            return RedirectToAction("Dashboard", "Dashboard");
        }
    }

    // ========== ViewModels ==========
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [MinLength(6, ErrorMessage = "Confirm Password must be at least 6 characters.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }

    public class SignInViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }

    public class PreferencesViewModel
    {
        [Required]
        public int Age { get; set; }

        [Required]
        public decimal Capital { get; set; }

        [Required]
        public string RiskTolerance { get; set; } // "Bad", "Ok", "Good"

        [Required]
        public string Priority1 { get; set; } // "Buzz" | "Financials" | "GlobalTrend"

        [Required]
        public string Priority2 { get; set; }

        [Required]
        public string Priority3 { get; set; }
    }
}
