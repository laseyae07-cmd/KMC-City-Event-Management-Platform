using KMCEventPlatform.Client.Models;

namespace KMCEventPlatform.Client.Services
{
    public interface IAuthApiService
    {
        Task<AuthServiceResult> RegisterAsync(
            RegisterViewModel registerViewModel
        );

        Task<AuthServiceResult> LoginAsync(
            LoginViewModel loginViewModel
        );
    }
}