using KMCEventPlatform.API.DTOs;
using KMCEventPlatform.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMCEventPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/Auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        [ProducesResponseType(
            typeof(AuthResponseDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<AuthResponseDto>> Register(
            RegisterDto registerDto)
        {
            bool emailExists =
                await _authService.EmailExistsAsync(registerDto.Email);

            if (emailExists)
            {
                return Conflict(new
                {
                    message =
                        "An account already exists with this email address."
                });
            }

            var response =
                await _authService.RegisterAsync(registerDto);

            return StatusCode(
                StatusCodes.Status201Created,
                response
            );
        }

        // POST: api/Auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType(
            typeof(AuthResponseDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login(
            LoginDto loginDto)
        {
            var response =
                await _authService.LoginAsync(loginDto);

            if (response == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email address or password."
                });
            }

            return Ok(response);
        }
    }
}