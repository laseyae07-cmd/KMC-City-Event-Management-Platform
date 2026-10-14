using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KMCEventPlatform.API.DTOs;
using KMCEventPlatform.API.Interfaces;
using KMCEventPlatform.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace KMCEventPlatform.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            return user != null;
        }

        public async Task<AuthResponseDto> RegisterAsync(
            RegisterDto registerDto)
        {
            var user = new User
            {
                FullName = registerDto.FullName.Trim(),
                Email = registerDto.Email.Trim().ToLowerInvariant(),
                PhoneNumber = registerDto.PhoneNumber?.Trim(),
                Role = registerDto.Role,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                registerDto.Password
            );

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return CreateAuthResponse(user);
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.GetByEmailAsync(
                loginDto.Email
            );

            if (user == null)
            {
                return null;
            }

            var verificationResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    loginDto.Password
                );

            if (verificationResult == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return CreateAuthResponse(user);
        }

        private AuthResponseDto CreateAuthResponse(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");

            string key = jwtSection["Key"]
                ?? throw new InvalidOperationException(
                    "JWT key is missing."
                );

            string issuer = jwtSection["Issuer"]
                ?? throw new InvalidOperationException(
                    "JWT issuer is missing."
                );

            string audience = jwtSection["Audience"]
                ?? throw new InvalidOperationException(
                    "JWT audience is missing."
                );

            int expiryMinutes = int.TryParse(
                jwtSection["ExpiryMinutes"],
                out int configuredMinutes
            )
                ? configuredMinutes
                : 60;

            DateTime expiresAt =
                DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserId.ToString()
                ),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()
                )
            };

            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key)
            );

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var jwtToken = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials
            );

            string token =
                new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return new AuthResponseDto
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                Token = token,
                ExpiresAt = expiresAt
            };
        }
    }
}