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
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        // GET: api/Events
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(
            typeof(IEnumerable<EventDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetEvents()
        {
            var events = await _eventRepository.GetAllAsync();

            return Ok(events.Select(MapToDto));
        }

        // GET: api/Events/search
        [AllowAnonymous]
        [HttpGet("search")]
        [ProducesResponseType(
            typeof(IEnumerable<EventDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EventDto>>> SearchEvents(
            [FromQuery] EventSearchDto searchDto)
        {
            var events = await _eventRepository.SearchAsync(searchDto);

            return Ok(events.Select(MapToDto));
        }

        // GET: api/Events/1
        [AllowAnonymous]
        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(EventDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventDto>> GetEvent(int id)
        {
            var cityEvent =
                await _eventRepository.GetByIdAsync(id);

            if (cityEvent == null)
            {
                return NotFound(new
                {
                    message = "Event was not found."
                });
            }

            return Ok(MapToDto(cityEvent));
        }

        // GET: api/Events/my-events
        [Authorize(Roles = "Organizer")]
        [HttpGet("my-events")]
        [ProducesResponseType(
            typeof(IEnumerable<EventDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<EventDto>>> GetMyEvents()
        {
            if (!TryGetCurrentUserId(out int organizerId))
            {
                return Unauthorized(new
                {
                    message = "The user ID is missing from the token."
                });
            }

            var events =
                await _eventRepository.GetByOrganizerIdAsync(organizerId);

            return Ok(events.Select(MapToDto));
        }

        // POST: api/Events
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ProducesResponseType(
            typeof(EventDto),
            StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<EventDto>> CreateEvent(
            CreateEventDto createEventDto)
        {
            if (!TryGetCurrentUserId(out int organizerId))
            {
                return Unauthorized(new
                {
                    message = "The user ID is missing from the token."
                });
            }

            bool eventTypeExists =
                await _eventRepository.EventTypeExistsAsync(
                    createEventDto.EventTypeId
                );

            if (!eventTypeExists)
            {
                return BadRequest(new
                {
                    message = "The selected event type does not exist."
                });
            }

            if (createEventDto.EventDate.Date < DateTime.Today)
            {
                return BadRequest(new
                {
                    message = "The event date cannot be in the past."
                });
            }

            if (createEventDto.EndTime <= createEventDto.StartTime)
            {
                return BadRequest(new
                {
                    message =
                        "The end time must be later than the start time."
                });
            }

            var cityEvent = new CityEvent
            {
                Title = createEventDto.Title.Trim(),
                Description = createEventDto.Description.Trim(),
                EventDate = createEventDto.EventDate.Date,
                StartTime = createEventDto.StartTime,
                EndTime = createEventDto.EndTime,
                Venue = createEventDto.Venue.Trim(),
                MaximumParticipants =
                    createEventDto.MaximumParticipants,
                EventTypeId = createEventDto.EventTypeId,
                OrganizerId = organizerId,
                Status = "Upcoming",
                CreatedAt = DateTime.UtcNow
            };

            await _eventRepository.AddAsync(cityEvent);
            await _eventRepository.SaveChangesAsync();

            var createdEvent =
                await _eventRepository.GetByIdAsync(cityEvent.EventId);

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = cityEvent.EventId },
                MapToDto(createdEvent!)
            );
        }

        // PUT: api/Events/1
        [Authorize(Roles = "Organizer")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(
            typeof(EventDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventDto>> UpdateEvent(
            int id,
            UpdateEventDto updateEventDto)
        {
            if (!TryGetCurrentUserId(out int organizerId))
            {
                return Unauthorized(new
                {
                    message = "The user ID is missing from the token."
                });
            }

            var cityEvent =
                await _eventRepository.GetByIdAsync(
                    id,
                    trackChanges: true
                );

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
                            "You can only update events that you created."
                    }
                );
            }

            bool eventTypeExists =
                await _eventRepository.EventTypeExistsAsync(
                    updateEventDto.EventTypeId
                );

            if (!eventTypeExists)
            {
                return BadRequest(new
                {
                    message = "The selected event type does not exist."
                });
            }

            if (updateEventDto.EventDate.Date < DateTime.Today)
            {
                return BadRequest(new
                {
                    message = "The event date cannot be in the past."
                });
            }

            if (updateEventDto.EndTime <= updateEventDto.StartTime)
            {
                return BadRequest(new
                {
                    message =
                        "The end time must be later than the start time."
                });
            }

            cityEvent.Title = updateEventDto.Title.Trim();
            cityEvent.Description = updateEventDto.Description.Trim();
            cityEvent.EventDate = updateEventDto.EventDate.Date;
            cityEvent.StartTime = updateEventDto.StartTime;
            cityEvent.EndTime = updateEventDto.EndTime;
            cityEvent.Venue = updateEventDto.Venue.Trim();
            cityEvent.MaximumParticipants =
                updateEventDto.MaximumParticipants;
            cityEvent.EventTypeId = updateEventDto.EventTypeId;
            cityEvent.Status = updateEventDto.Status;
            cityEvent.UpdatedAt = DateTime.UtcNow;

            await _eventRepository.SaveChangesAsync();

            var updatedEvent =
                await _eventRepository.GetByIdAsync(id);

            return Ok(MapToDto(updatedEvent!));
        }

        // DELETE: api/Events/1
        [Authorize(Roles = "Organizer")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            if (!TryGetCurrentUserId(out int organizerId))
            {
                return Unauthorized(new
                {
                    message = "The user ID is missing from the token."
                });
            }

            var cityEvent =
                await _eventRepository.GetByIdAsync(
                    id,
                    trackChanges: true
                );

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
                            "You can only delete events that you created."
                    }
                );
            }

            _eventRepository.Delete(cityEvent);
            await _eventRepository.SaveChangesAsync();

            return NoContent();
        }

        private bool TryGetCurrentUserId(out int userId)
        {
            string? userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            return int.TryParse(userIdValue, out userId);
        }

        private static EventDto MapToDto(CityEvent cityEvent)
        {
            return new EventDto
            {
                EventId = cityEvent.EventId,
                Title = cityEvent.Title,
                Description = cityEvent.Description,
                EventDate = cityEvent.EventDate,
                StartTime = cityEvent.StartTime,
                EndTime = cityEvent.EndTime,
                Venue = cityEvent.Venue,
                MaximumParticipants =
                    cityEvent.MaximumParticipants,

                RegisteredParticipants =
                    cityEvent.Registrations.Count(
                        registration =>
                            registration.Status == "Registered"
                    ),

                Status = cityEvent.Status,
                EventTypeId = cityEvent.EventTypeId,

                EventTypeName =
                    cityEvent.EventType?.TypeName ?? string.Empty,

                OrganizerId = cityEvent.OrganizerId,

                OrganizerName =
                    cityEvent.Organizer?.FullName ?? string.Empty,

                CreatedAt = cityEvent.CreatedAt,
                UpdatedAt = cityEvent.UpdatedAt
            };
        }
    }
}