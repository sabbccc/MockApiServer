using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MockApiServer.Models.ViewModels;
using MockApiServer.Services;
using System.Security.Claims;

namespace MockApiServer.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region Login / Logout / Register
        // ===========================
        // Login / Logout / Register
        // ===========================
        [HttpGet]
        public IActionResult Login(bool timeout = false)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                // User is already logged in, redirect to dashboard
                return RedirectToAction("Index", "Home");
            }

            //Session Timeout Message
            if (timeout) TempData["SessionTimeout"] = "Your session has timed out. Please log in again.";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.AuthenticateAsync(model.Username ?? "", model.Password ?? "");
            if (user != null)
            {
                // Create Claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                    new Claim("FullName", user.Name ?? string.Empty),
                    new Claim("MobileNo", user.MobileNo ?? string.Empty),
                    new Claim("UserType", "Administrator"),
                };

                // Create Identity and Principal
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Set cookie options based on RememberMe
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe,
                    ExpiresUtc = model.RememberMe == true
                        ? DateTimeOffset.Now.AddDays(7)       // Remember Me = 7 days
                        : DateTimeOffset.Now.AddMinutes(20)   // Normal session = 20 mins
                };

                // Sign In
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            TempData["Failed"] = "Invalid username or password";
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            // Clear Authentication Cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["Success"] = "Logged out successfully.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        [Authorize]
        public IActionResult Register()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _userService.RegisterAsync(model);
            TempData["Success"] = "User registered successfully!";
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        #endregion

        #region CRUD: Index / Create / Edit / Delete
        // ===========================
        // CRUD: Index / Create / Edit / Delete
        // ===========================
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _userService.AddAsync(model);
            TempData["Success"] = "User created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _userService.UpdateAsync(model);
            TempData["Success"] = "User updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteAsync(id);
            TempData["Success"] = "User deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
