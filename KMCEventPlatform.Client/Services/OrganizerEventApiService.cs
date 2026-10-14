using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using KMCEventPlatform.Client.Models;

namespace KMCEventPlatform.Client.Services
{
    public class OrganizerEventApiService
        : IOrganizerEventApiService
    {
        private readonly HttpClient _httpClient;

        public OrganizerEventApiService(
            HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EventViewModel>>
            GetMyEventsAsync(string token)
        {
            using var request = CreateRequest(
                HttpMethod.Get,
                "api/Events/my-events",
                token
            );

            HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var events =
                await response.Content.ReadFromJsonAsync<
                    List<EventViewModel>>();

            return events ?? new List<EventViewModel>();
        }

        public async Task<List<RegistrationViewModel>>
            GetEventRegistrationsAsync(
                int eventId,
                string token)
        {
            using var request = CreateRequest(
                HttpMethod.Get,
                $"api/Registrations/event/{eventId}",
                token
            );

            HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var registrations =
                await response.Content.ReadFromJsonAsync<
                    List<RegistrationViewModel>>();

            return registrations
                ?? new List<RegistrationViewModel>();
        }

        public async Task<OrganizerEventServiceResult>
            CreateEventAsync(
                OrganizerEventFormViewModel eventViewModel,
                string token)
        {
            using var request = CreateRequest(
                HttpMethod.Post,
                "api/Events",
                token
            );

            request.Content = JsonContent.Create(new
            {
                eventViewModel.Title,
                eventViewModel.Description,
                eventViewModel.EventDate,
                eventViewModel.StartTime,
                eventViewModel.EndTime,
                eventViewModel.Venue,
                eventViewModel.MaximumParticipants,
                eventViewModel.EventTypeId
            });

            HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            return await CreateServiceResultAsync(response);
        }

        public async Task<OrganizerEventServiceResult>
            UpdateEventAsync(
                int eventId,
                OrganizerEventFormViewModel eventViewModel,
                string token)
        {
            using var request = CreateRequest(
                HttpMethod.Put,
                $"api/Events/{eventId}",
                token
            );

            request.Content = JsonContent.Create(new
            {
                eventViewModel.Title,
                eventViewModel.Description,
                eventViewModel.EventDate,
                eventViewModel.StartTime,
                eventViewModel.EndTime,
                eventViewModel.Venue,
                eventViewModel.MaximumParticipants,
                eventViewModel.EventTypeId,
                eventViewModel.Status
            });

            HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            return await CreateServiceResultAsync(response);
        }

        public async Task<ApiActionResult>
            DeleteEventAsync(
                int eventId,
                string token)
        {
            using var request = CreateRequest(
                HttpMethod.Delete,
                $"api/Events/{eventId}",
                token
            );

            HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return new ApiActionResult
                {
                    Succeeded = true
                };
            }

            return new ApiActionResult
            {
                Succeeded = false,
                ErrorMessage =
                    await ReadErrorMessageAsync(response)
            };
        }

        private static HttpRequestMessage CreateRequest(
            HttpMethod method,
            string url,
            string token)
        {
            var request =
                new HttpRequestMessage(method, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token
                );

            return request;
        }

        private static async Task<OrganizerEventServiceResult>
            CreateServiceResultAsync(
                HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var cityEvent =
                    await response.Content.ReadFromJsonAsync<
                        EventViewModel>();

                return new OrganizerEventServiceResult
                {
                    Succeeded = true,
                    Event = cityEvent
                };
            }

            return new OrganizerEventServiceResult
            {
                Succeeded = false,
                ErrorMessage =
                    await ReadErrorMessageAsync(response)
            };
        }

        private static async Task<string>
            ReadErrorMessageAsync(
                HttpResponseMessage response)
        {
            try
            {
                var error =
                    await response.Content.ReadFromJsonAsync<
                        ApiErrorViewModel>();

                if (!string.IsNullOrWhiteSpace(
                    error?.Message))
                {
                    return error.Message;
                }
            }
            catch (JsonException)
            {
                // Use the status-based message below.
            }
            catch (NotSupportedException)
            {
                // Use the status-based message below.
            }

            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized =>
                    "Your login session has expired.",

                HttpStatusCode.Forbidden =>
                    "You are not permitted to manage this event.",

                HttpStatusCode.NotFound =>
                    "The event was not found.",

                HttpStatusCode.BadRequest =>
                    "The event information is invalid.",

                _ =>
                    "The request could not be completed."
            };
        }
    }
}