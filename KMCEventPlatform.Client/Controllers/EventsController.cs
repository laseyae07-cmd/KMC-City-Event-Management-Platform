using KMCEventPlatform.Client.Services;
using Microsoft.AspNetCore.Mvc;

namespace KMCEventPlatform.Client.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventApiService _eventApiService;

        public EventsController(
            IEventApiService eventApiService)
        {
            _eventApiService = eventApiService;
        }

        public async Task<IActionResult> Index(
            string? title,
            DateTime? date,
            int? eventTypeId,
            string? venue,
            string? status)
        {
            var viewModel =
                new Models.EventSearchViewModel
                {
                    Title = title,
                    Date = date,
                    EventTypeId = eventTypeId,
                    Venue = venue,
                    Status = status
                };

            try
            {
                viewModel.EventTypes =
                    await _eventApiService
                        .GetEventTypesAsync();

                viewModel.Events =
                    await _eventApiService
                        .SearchEventsAsync(
                            title,
                            date,
                            eventTypeId,
                            venue,
                            status
                        );
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "The event service is currently unavailable.";
            }

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var cityEvent =
                    await _eventApiService
                        .GetEventByIdAsync(id);

                if (cityEvent == null)
                {
                    return NotFound();
                }

                return View(cityEvent);
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "The event service is currently unavailable.";

                return View();
            }
        }
    }
}