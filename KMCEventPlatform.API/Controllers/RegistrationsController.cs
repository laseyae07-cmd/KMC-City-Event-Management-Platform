using System.Security.Claims;
using KMCEventPlatform.API.DTOs;
using KMCEventPlatform.API.Interfaces;
using KMCEventPlatform.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMCEventPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistrationRepository
            _registrationRepository;

        private readonly IEventRepository _eventRepository;

        public RegistrationsController(
            IRegistrationRepository registrationRepository,
            IEventRepository eventRepository)
        {
            _registrationRepository = registrationRepository;
            _eventRepository = eventRepository;
        }

        // POST: api/Registrations
        [Authorize(Roles = "Participant")]
        [HttpPost]
        [ProducesResponseType(
            typeof(RegistrationDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<RegistrationDto>>
            RegisterForEvent(
                CreateRegistrationDto createRegistrationDto)
        {
            if (!TryGetCurrentUserId(out int participantId))
            {
                return Unauthorized(new
                {
                    message =
                        "The user ID is missing from the token."
                });
            }

            var cityEvent =
                await _eventRepository.GetByIdAsync(
                    createRegistrationDto.EventId
                );

            if (cityEvent == null)
            {
                return NotFound(new
                {
                    message = "Event was not found."
                });
            }

            if (cityEvent.Status != "Upcoming")
            {
                return BadRequest(new
                {
                    message =
                        "Registration is only available for upcoming events."
                });
            }

            if (cityEvent.EventDate.Date < DateTime.Today)
            {
                return BadRequest(new
                {
                    message =
                        "Registration is closed because the event has passed."
                });
            }

            var existingRegistration =
                await _registrationRepository
                    .GetByEventAndParticipantAsync(
                        cityEvent.EventId,
                        participantId,
                        trackChanges: true
                    );

            if (existingRegistration?.Status == "Registered")
            {
                return Conflict(new
                {
                    message =
                        "You are already registered for this event."
                });
            }

            int activeRegistrations =
                await _registrationRepository
                    .CountActiveRegistrationsAsync(
                        cityEvent.EventId
                    );

            if (activeRegistrations >=
                cityEvent.MaximumParticipants)
            {
                return Conflict(new
                {
                    message =
                        "Registration is closed because the event is full."
                });
            }

            // Reactivate a previously cancelled registration.
            if (existingRegistration != null)
            {
                existingRegistration.Status = "Registered";
                existingRegistration.RegistrationDate =
                    DateTime.UtcNow;

                await _registrationRepository.SaveChangesAsync();

                var reactivatedRegistration =
                    await _registrationRepository.GetByIdAsync(
                        existingRegistration.RegistrationId
                    );

                return Ok(
                    MapToDto(reactivatedRegistration!)
                );
            }

            var registration = new Registration
            {
                EventId = cityEvent.EventId,
                ParticipantId = participantId,
                RegistrationDate = DateTime.UtcNow,
                Status = "Registered"
            };

            await _registrationRepository.AddAsync(registration);
            await _registrationRepository.SaveChangesAsync();

            var createdRegistration =
                await _registrationRepository.GetByIdAsync(
                    registration.RegistrationId
                );

            return StatusCode(
                StatusCodes.Status201Created,
                MapToDto(createdRegistration!)
            );
        }

        // GET: api/Registrations/my-registrations
        [Authorize(Roles = "Participant")]
        [HttpGet("my-registrations")]
        [ProducesResponseType(
            typeof(IEnumerable<RegistrationDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<
            ActionResult<IEnumerable<RegistrationDto>>>
            GetMyRegistrations()
        {
            if (!TryGetCurrentUserId(out int participantId))
            {
                return Unauthorized(new
                {
                    message =
                        "The user ID is missing from the token."
                });
            }

            var registrations =
                await _registrationRepository
                    .GetByParticipantIdAsync(participantId);

            return Ok(registrations.Select(MapToDto));
        }

        // GET: api/Registrations/event/1
        [Authorize(Roles = "Organizer")]
        [HttpGet("event/{eventId:int}")]
        [ProducesResponseType(
            typeof(IEnumerable<RegistrationDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<
            ActionResult<IEnumerable<RegistrationDto>>>
            GetEventRegistrations(int eventId)
        {
            if (!TryGetCurrentUserId(out int organizerId))
            {
                return Unauthorized(new
                {
                    message =
                        "The user ID is missing from the token."
                });
            }

            var cityEvent =
                await _eventRepository.GetByIdAsync(eventId);

            if (cityEvent == null)
            {
                return NotFound(new
                {
                    message = "Event was not found."
                });
            }

            if (cityEvent.OrganizerId != organizerId)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message =
                            "You can only view registrations for events that you created."
                    }
                );
            }

            var registrations =
                await _registrationRepository
                    .GetByEventIdAsync(eventId);

            return Ok(registrations.Select(MapToDto));
        }

        // DELETE: api/Registrations/1
        [Authorize(Roles = "Participant")]
        [HttpDelete("{registrationId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelRegistration(
            int registrationId)
        {
            if (!TryGetCurrentUserId(out int participantId))
            {
                return Unauthorized(new
                {
                    message =
                        "The user ID is missing from the token."
                });
            }

            var registration =
                await _registrationRepository.GetByIdAsync(
                    registrationId,
                    trackChanges: true
                );

            if (registration == null)
            {
                return NotFound(new
                {
                    message = "Registration was not found."
                });
            }

            if (registration.ParticipantId != participantId)
            {
                return StatusCode(
                    StatusCodes.Status403Forbidden,
                    new
                    {
                        message =
                            "You can only cancel your own registrations."
                    }
                );
            }

            if (registration.Status == "Cancelled")
            {
                return BadRequest(new
                {
                    message =
                        "This registration has already been cancelled."
                });
            }

            registration.Status = "Cancelled";

            await _registrationRepository.SaveChangesAsync();

            return NoContent();
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            string? userIdValue =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier
                );

            return int.TryParse(userIdValue, out userId);
        }

        private static RegistrationDto MapToDto(
            Registration registration)
        {
            return new RegistrationDto
            {
                RegistrationId =
                    registration.RegistrationId,

                EventId = registration.EventId,

                EventTitle =
                    registration.Event?.Title
                    ?? string.Empty,

                EventDate =
                    registration.Event?.EventDate
                    ?? default,

                Venue =
                    registration.Event?.Venue
                    ?? string.Empty,

                ParticipantId =
                    registration.ParticipantId,

                ParticipantName =
                    registration.Participant?.FullName
                    ?? string.Empty,

                ParticipantEmail =
                    registration.Participant?.Email
                    ?? string.Empty,

                RegistrationDate =
                    registration.RegistrationDate,

                Status = registration.Status
            };
        }
    }
}