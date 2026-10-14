using KMCEventPlatform.Client.Models;
using KMCEventPlatform.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace KMCEventPlatform.Client.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly IRegistrationApiService
            _registrationApiService;

        public RegistrationsController(
            IRegistrationApiService registrationApiService)
        {
            _registrationApiService =
                registrationApiService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int eventId)
        {
            string? token =
                HttpContext.Session.GetString("JwtToken");

            string? role =
                HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["ErrorMessage"] =
                    "Please log in before registering for an event.";

                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            if (role != "Participant")
            {
                TempData["ErrorMessage"] =
                    "Only participant accounts can register for events.";

                return RedirectToAction(
                    "Details",
                    "Events",
                    new { id = eventId }
                );
            }

            try
            {
                RegistrationServiceResult result =
                    await _registrationApiService
                        .RegisterForEventAsync(
                            eventId,
                            token
                        );

                if (!result.Succeeded)
                {
                    TempData["ErrorMessage"] =
                        result.ErrorMessage;
                }
                else
                {
                    TempData["SuccessMessage"] =
                        "You have successfully registered for the event.";
                }
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "The registration service is currently unavailable.";
            }

            return RedirectToAction(
                "Details",
                "Events",
                new { id = eventId }
            );
        }

        [HttpGet]
        public async Task<IActionResult> MyRegistrations()
        {
            string? token =
                HttpContext.Session.GetString("JwtToken");

            string? role =
                HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            if (role != "Participant")
            {
                TempData["ErrorMessage"] =
                    "This page is available only to participants.";

                return RedirectToAction(
                    "Index",
                    "Events"
                );
            }

            try
            {
                var registrations =
                    await _registrationApiService
                        .GetMyRegistrationsAsync(token);

                return View(registrations);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "The registration service is currently unavailable.";

                return View(
                    new List<RegistrationViewModel>()
                );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(
            int registrationId)
        {
            string? token =
                HttpContext.Session.GetString("JwtToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            try
            {
                ApiActionResult result =
                    await _registrationApiService
                        .CancelRegistrationAsync(
                            registrationId,
                            token
                        );

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] =
                        "The registration was cancelled successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] =
                        result.ErrorMessage;
                }
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "The registration service is currently unavailable.";
            }

            return RedirectToAction(
                nameof(MyRegistrations)
            );
        }
    }
}