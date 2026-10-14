using KMCEventPlatform.API.DTOs;

namespace KMCEventPlatform.API.Interfaces
{
    public interface IAuthService
    {
        Task<bool> EmailExistsAsync(string email);

        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);

        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
    }
}