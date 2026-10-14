using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using KMCEventPlatform.Client.Models;

namespace KMCEventPlatform.Client.Services
{
    public class RegistrationApiService
        : IRegistrationApiService
    {
        private readonly HttpClient _httpClient;

        public RegistrationApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<RegistrationServiceResult>
            RegisterForEventAsync(
                int eventId,
                string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "api/Registrations"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token
                );

            request.Content = JsonContent.Create(new
            {
                eventId
            });

            HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var registration =
                    await response.Content
                        .ReadFromJsonAsync<
                            RegistrationViewModel>();

                return new RegistrationServiceResult
                {
                    Succeeded = true,
                    Registration = registration
                };
            }

            return new RegistrationServiceResult
            {
                Succeeded = false,
                ErrorMessage =
                    await ReadErrorMessageAsync(response)
            };
        }

        public async Task<List<RegistrationViewModel>>
            GetMyRegistrationsAsync(string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Get,
                "api/Registrations/my-registrations"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    token
                );

            HttpResponseMessage response =
                await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var registrations =
                await response.Content
                    .ReadFromJsonAsync<
                        List<RegistrationViewModel>>();

            return registrations
                ?? new List<RegistrationViewModel>();
        }

        public async Task<ApiActionResult>
            CancelRegistrationAsync(
                int registrationId,
                string token)
        {
            using var request = new HttpRequestMessage(
                HttpMethod.Delete,
                $"api/Registrations/{registrationId}"
            );

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
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

        private static async Task<string>
            ReadErrorMessageAsync(
                HttpResponseMessage response)
        {
            try
            {
                var error =
                    await response.Content
                        .ReadFromJsonAsync<ApiErrorViewModel>();

                if (!string.IsNullOrWhiteSpace(
                    error?.Message))
                {
                    return error.Message;
                }
            }
            catch (JsonException)
            {
                // Use the default message below.
            }
            catch (NotSupportedException)
            {
                // Use the default message below.
            }

            return "The registration request could not be completed.";
        }
    }
}