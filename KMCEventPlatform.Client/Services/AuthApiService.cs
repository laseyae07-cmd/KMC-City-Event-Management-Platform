using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using KMCEventPlatform.Client.Models;

namespace KMCEventPlatform.Client.Services
{
    public class AuthApiService : IAuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthServiceResult> RegisterAsync(
            RegisterViewModel registerViewModel)
        {
            var request = new
            {
                registerViewModel.FullName,
                registerViewModel.Email,
                registerViewModel.Password,
                registerViewModel.PhoneNumber,
                registerViewModel.Role
            };

            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "api/Auth/register",
                    request
                );

            return await CreateResultAsync(response);
        }

        public async Task<AuthServiceResult> LoginAsync(
            LoginViewModel loginViewModel)
        {
            var request = new
            {
                loginViewModel.Email,
                loginViewModel.Password
            };

            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "api/Auth/login",
                    request
                );

            return await CreateResultAsync(response);
        }

        private static async Task<AuthServiceResult>
            CreateResultAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var authResponse =
                    await response.Content
                        .ReadFromJsonAsync<AuthResponseViewModel>();

                if (authResponse == null)
                {
                    return new AuthServiceResult
                    {
                        Succeeded = false,
                        ErrorMessage =
                            "The authentication response was invalid."
                    };
                }

                return new AuthServiceResult
                {
                    Succeeded = true,
                    Response = authResponse
                };
            }

            string errorMessage =
                await ReadErrorMessageAsync(response);

            return new AuthServiceResult
            {
                Succeeded = false,
                ErrorMessage = errorMessage
            };
        }

        private static async Task<string>
            ReadErrorMessageAsync(HttpResponseMessage response)
        {
            try
            {
                var error =
                    await response.Content
                        .ReadFromJsonAsync<ApiErrorViewModel>();

                if (!string.IsNullOrWhiteSpace(error?.Message))
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
                HttpStatusCode.Conflict =>
                    "An account already exists with this email.",

                HttpStatusCode.Unauthorized =>
                    "Invalid email address or password.",

                HttpStatusCode.BadRequest =>
                    "The submitted information is invalid.",

                _ => "The request could not be completed."
            };
        }
    }
}