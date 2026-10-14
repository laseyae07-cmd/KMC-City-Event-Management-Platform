using System.Net;
using System.Net.Http.Json;
using KMCEventPlatform.Client.Models;

namespace KMCEventPlatform.Client.Services
{
    public class EventApiService : IEventApiService
    {
        private readonly HttpClient _httpClient;

        public EventApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            var events =
                await _httpClient.GetFromJsonAsync<
                    List<EventViewModel>>("api/Events");

            return events ?? new List<EventViewModel>();
        }

        public async Task<EventViewModel?> GetEventByIdAsync(int id)
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync($"api/Events/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<EventViewModel>();
        }

        public async Task<List<EventTypeViewModel>>
            GetEventTypesAsync()
        {
            var eventTypes =
                await _httpClient.GetFromJsonAsync<
                    List<EventTypeViewModel>>("api/EventTypes");

            return eventTypes
                ?? new List<EventTypeViewModel>();
        }

        public async Task<List<EventViewModel>>
            SearchEventsAsync(
                string? title,
                DateTime? date,
                int? eventTypeId,
                string? venue,
                string? status)
        {
            var queryParameters = new List<string>();

            if (!string.IsNullOrWhiteSpace(title))
            {
                queryParameters.Add(
                    $"title={Uri.EscapeDataString(title.Trim())}"
                );
            }

            if (date.HasValue)
            {
                queryParameters.Add(
                    $"date={date.Value:yyyy-MM-dd}"
                );
            }

            if (eventTypeId.HasValue)
            {
                queryParameters.Add(
                    $"eventTypeId={eventTypeId.Value}"
                );
            }

            if (!string.IsNullOrWhiteSpace(venue))
            {
                queryParameters.Add(
                    $"venue={Uri.EscapeDataString(venue.Trim())}"
                );
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                queryParameters.Add(
                    $"status={Uri.EscapeDataString(status.Trim())}"
                );
            }

            string url = "api/Events/search";

            if (queryParameters.Count > 0)
            {
                url += "?" + string.Join("&", queryParameters);
            }

            var events =
                await _httpClient.GetFromJsonAsync<
                    List<EventViewModel>>(url);

            return events ?? new List<EventViewModel>();
        }
    }
}