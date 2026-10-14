using KMCEventPlatform.Client.Models;
using KMCEventPlatform.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace KMCEventPlatform.Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authApiService;

        public AccountController(
            IAuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel registerViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(registerViewModel);
            }

            try
            {
                AuthServiceResult result =
                    await _authApiService.RegisterAsync(
                        registerViewModel
                    );

                if (!result.Succeeded ||
                    result.Response == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        result.ErrorMessage
                    );

                    return View(registerViewModel);
                }

                StoreUserSession(result.Response);

                TempData["SuccessMessage"] =
                    "Your account was created successfully.";

                return RedirectToAction(
                    "Index",
                    "Events"
                );
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The authentication service is unavailable."
                );

                return View(registerViewModel);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(loginViewModel);
            }

            try
            {
                AuthServiceResult result =
                    await _authApiService.LoginAsync(
                        loginViewModel
                    );

                if (!result.Succeeded ||
                    result.Response == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        result.ErrorMessage
                    );

                    return View(loginViewModel);
                }

                StoreUserSession(result.Response);

                TempData["SuccessMessage"] =
                    "You have logged in successfully.";

                return RedirectToAction(
                    "Index",
                    "Events"
                );
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The authentication service is unavailable."
                );

                return View(loginViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Index",
                "Events"
            );
        }

        private void StoreUserSession(
            AuthResponseViewModel authResponse)
        {
            HttpContext.Session.SetString(
                "JwtToken",
                authResponse.Token
            );

            HttpContext.Session.SetInt32(
                "UserId",
                authResponse.UserId
            );

            HttpContext.Session.SetString(
                "FullName",
                authResponse.FullName
            );

            HttpContext.Session.SetString(
                "UserEmail",
                authResponse.Email
            );

            HttpContext.Session.SetString(
                "UserRole",
                authResponse.Role
            );

            HttpContext.Session.SetString(
                "TokenExpiry",
                authResponse.ExpiresAt.ToString("O")
            );
        }
    }
}