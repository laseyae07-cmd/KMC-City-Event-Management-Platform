using KMCEventPlatform.API.DTOs;
using KMCEventPlatform.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMCEventPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypesController : ControllerBase
    {
        private readonly IEventTypeRepository _eventTypeRepository;

        public EventTypesController(
            IEventTypeRepository eventTypeRepository)
        {
            _eventTypeRepository = eventTypeRepository;
        }

        // GET: api/EventTypes
        [HttpGet]
        [ProducesResponseType(
            typeof(IEnumerable<EventTypeDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EventTypeDto>>>
            GetEventTypes()
        {
            var eventTypes =
                await _eventTypeRepository.GetAllAsync();

            var eventTypeDtos = eventTypes.Select(eventType =>
                new EventTypeDto
                {
                    EventTypeId = eventType.EventTypeId,
                    TypeName = eventType.TypeName,
                    Description = eventType.Description
                });

            return Ok(eventTypeDtos);
        }

        // GET: api/EventTypes/1
        [HttpGet("{id:int}")]
        [ProducesResponseType(
            typeof(EventTypeDto),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventTypeDto>>
            GetEventType(int id)
        {
            var eventType =
                await _eventTypeRepository.GetByIdAsync(id);

            if (eventType == null)
            {
                return NotFound(new
                {
                    message = "Event type was not found."
                });
            }

            var eventTypeDto = new EventTypeDto
            {
                EventTypeId = eventType.EventTypeId,
                TypeName = eventType.TypeName,
                Description = eventType.Description
            };

            return Ok(eventTypeDto);
        }
    }
}