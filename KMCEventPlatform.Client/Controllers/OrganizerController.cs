using KMCEventPlatform.Client.Models;
using KMCEventPlatform.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace KMCEventPlatform.Client.Controllers
{
    public class OrganizerController : Controller
    {
        private readonly IOrganizerEventApiService
            _organizerEventApiService;

        private readonly IEventApiService
            _eventApiService;

        public OrganizerController(
            IOrganizerEventApiService organizerEventApiService,
            IEventApiService eventApiService)
        {
            _organizerEventApiService =
                organizerEventApiService;

            _eventApiService = eventApiService;
        }

        // Display events created by the logged-in organizer.
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            string? token = GetOrganizerToken();

            if (token == null)
            {
                return RedirectToAccountOrEvents();
            }

            try
            {
                var events =
                    await _organizerEventApiService
                        .GetMyEventsAsync(token);

                return View(events);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "The event management service is unavailable.";

                return View(new List<EventViewModel>());
            }
        }

        // Display participants registered for an organizer's event.
        [HttpGet]
        public async Task<IActionResult> Registrations(int id)
        {
            string? token = GetOrganizerToken();

            if (token == null)
            {
                return RedirectToAccountOrEvents();
            }

            try
            {
                EventViewModel? cityEvent =
                    await _eventApiService
                        .GetEventByIdAsync(id);

                if (cityEvent == null)
                {
                    return NotFound();
                }

                int? currentUserId =
                    HttpContext.Session.GetInt32("UserId");

                if (currentUserId != cityEvent.OrganizerId)
                {
                    TempData["ErrorMessage"] =
                        "You can only view participants for events that you created.";

                    return RedirectToAction(
                        nameof(Dashboard)
                    );
                }

                var registrations =
                    await _organizerEventApiService
                        .GetEventRegistrationsAsync(
                            id,
                            token
                        );

                ViewBag.EventTitle = cityEvent.Title;
                ViewBag.EventId = cityEvent.EventId;

                return View(registrations);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "The registration service is unavailable.";

                return RedirectToAction(
                    nameof(Dashboard)
                );
            }
        }

        // Display the create-event form.
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (GetOrganizerToken() == null)
            {
                return RedirectToAccountOrEvents();
            }

            var viewModel =
                new OrganizerEventFormViewModel
                {
                    EventDate = DateTime.Today,
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    MaximumParticipants = 100
                };

            try
            {
                await PopulateEventTypesAsync(viewModel);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Event types could not be loaded.";
            }

            return View(viewModel);
        }

        // Create a new event.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            OrganizerEventFormViewModel viewModel)
        {
            string? token = GetOrganizerToken();

            if (token == null)
            {
                return RedirectToAccountOrEvents();
            }

            if (!ModelState.IsValid)
            {
                await PopulateEventTypesSafelyAsync(viewModel);

                return View(viewModel);
            }

            try
            {
                OrganizerEventServiceResult result =
                    await _organizerEventApiService
                        .CreateEventAsync(
                            viewModel,
                            token
                        );

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        result.ErrorMessage
                    );

                    await PopulateEventTypesSafelyAsync(
                        viewModel
                    );

                    return View(viewModel);
                }

                TempData["SuccessMessage"] =
                    "The event was created successfully.";

                return RedirectToAction(
                    nameof(Dashboard)
                );
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The event management service is unavailable."
                );

                await PopulateEventTypesSafelyAsync(viewModel);

                return View(viewModel);
            }
        }

        // Display an organizer's event for editing.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (GetOrganizerToken() == null)
            {
                return RedirectToAccountOrEvents();
            }

            try
            {
                EventViewModel? cityEvent =
                    await _eventApiService
                        .GetEventByIdAsync(id);

                if (cityEvent == null)
                {
                    return NotFound();
                }

                int? currentUserId =
                    HttpContext.Session.GetInt32("UserId");

                if (currentUserId != cityEvent.OrganizerId)
                {
                    TempData["ErrorMessage"] =
                        "You can only edit events that you created.";

                    return RedirectToAction(
                        nameof(Dashboard)
                    );
                }

                var viewModel =
                    new OrganizerEventFormViewModel
                    {
                        EventId = cityEvent.EventId,
                        Title = cityEvent.Title,
                        Description = cityEvent.Description,
                        EventDate = cityEvent.EventDate,
                        StartTime = cityEvent.StartTime,
                        EndTime = cityEvent.EndTime,
                        Venue = cityEvent.Venue,
                        MaximumParticipants =
                            cityEvent.MaximumParticipants,
                        EventTypeId = cityEvent.EventTypeId,
                        Status = cityEvent.Status
                    };

                await PopulateEventTypesAsync(viewModel);

                return View(viewModel);
            }
            catch (HttpRequestException)
            {
                TempData["ErrorMessage"] =
                    "The event service is unavailable.";

                return RedirectToAction(
                    nameof(Dashboard)
                );
            }
        }

        // Update an event.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            OrganizerEventFormViewModel viewModel)
        {
            string? token = GetOrganizerToken();

            if (token == null)
            {
                return RedirectToAccountOrEvents();
            }

            if (id != viewModel.EventId)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateEventTypesSafelyAsync(viewModel);

                return View(viewModel);
            }

            try
            {
                OrganizerEventServiceResult result =
                    await _organizerEventApiService
                        .UpdateEventAsync(
                            id,
                            viewModel,
                            token
                        );

                if (!result.Succeeded)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        result.ErrorMessage
                    );

                    await PopulateEventTypesSafelyAsync(
                        viewModel
                    );

                    return View(viewModel);
                }

                TempData["SuccessMessage"] =
                    "The event was updated successfully.";

                return RedirectToAction(
                    nameof(Dashboard)
                );
            }
            catch (HttpRequestException)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The event management service is unavailable."
                );

                await PopulateEventTypesSafelyAsync(viewModel);

                return View(viewModel);
            }
        }

        // Delete an event.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            string? token = GetOrganizerToken();

            if (token == null)
            {
                return RedirectToAccountOrEvents();
            }

            try
            {
                ApiActionResult result =
                    await _organizerEventApiService
                        .DeleteEventAsync(id, token);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] =
                        "The event was deleted successfully.";
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
                    "The event management service is unavailable.";
            }

            return RedirectToAction(
                nameof(Dashboard)
            );
        }

        private string? GetOrganizerToken()
        {
            string? token =
                HttpContext.Session.GetString("JwtToken");

            string? role =
                HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrWhiteSpace(token) ||
                role != "Organizer")
            {
                return null;
            }

            return token;
        }

        private IActionResult RedirectToAccountOrEvents()
        {
            bool loggedIn =
                !string.IsNullOrWhiteSpace(
                    HttpContext.Session.GetString(
                        "JwtToken"
                    )
                );

            if (!loggedIn)
            {
                return RedirectToAction(
                    "Login",
                    "Account"
                );
            }

            TempData["ErrorMessage"] =
                "This area is available only to event organizers.";

            return RedirectToAction(
                "Index",
                "Events"
            );
        }

        private async Task PopulateEventTypesAsync(
            OrganizerEventFormViewModel viewModel)
        {
            viewModel.EventTypes =
                await _eventApiService
                    .GetEventTypesAsync();
        }

        private async Task PopulateEventTypesSafelyAsync(
            OrganizerEventFormViewModel viewModel)
        {
            try
            {
                await PopulateEventTypesAsync(viewModel);
            }
            catch (HttpRequestException)
            {
                viewModel.EventTypes =
                    new List<EventTypeViewModel>();
            }
        }
    }
}