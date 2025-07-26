using AIStockRadar.Models; // Adjust namespace to your project
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using YourProjectName.Models;

namespace YourProjectName.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        // Inject the database context
        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Prefrences()
        {
            return View();
        }

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

            // Check if user exists (case-insensitive)
            var existingUser = _context.Users.FirstOrDefault(u => u.Email.ToLower() == model.Email.ToLower());
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email already registered.");
                return View(model);
            }

            // Create new user (store plain password for now - hash in real app)
            var user = new User
            {
                Email = model.Email,
                PasswordHash = model.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Prefrences", "Account");
        }

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

            return RedirectToAction("Dashboard", "Dashboard");
        }
    }

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
}
