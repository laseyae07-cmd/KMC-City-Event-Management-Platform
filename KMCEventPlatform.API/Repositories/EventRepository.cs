using KMCEventPlatform.API.Data;
using KMCEventPlatform.API.Interfaces;
using KMCEventPlatform.API.Models;
using Microsoft.EntityFrameworkCore;
using KMCEventPlatform.API.DTOs;

namespace KMCEventPlatform.API.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CityEvent>> GetAllAsync()
        {
            return await _context.Events
                .AsNoTracking()
                .Include(cityEvent => cityEvent.EventType)
                .Include(cityEvent => cityEvent.Organizer)
                .Include(cityEvent => cityEvent.Registrations)
                .OrderBy(cityEvent => cityEvent.EventDate)
                .ThenBy(cityEvent => cityEvent.StartTime)
                .ToListAsync();
        }

        public async Task<CityEvent?> GetByIdAsync(
            int id,
            bool trackChanges = false)
        {
            IQueryable<CityEvent> query = _context.Events
                .Include(cityEvent => cityEvent.EventType)
                .Include(cityEvent => cityEvent.Organizer)
                .Include(cityEvent => cityEvent.Registrations);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(
                cityEvent => cityEvent.EventId == id
            );
        }

        public async Task<IEnumerable<CityEvent>>
            GetByOrganizerIdAsync(int organizerId)
        {
            return await _context.Events
                .AsNoTracking()
                .Include(cityEvent => cityEvent.EventType)
                .Include(cityEvent => cityEvent.Organizer)
                .Include(cityEvent => cityEvent.Registrations)
                .Where(
                    cityEvent =>
                        cityEvent.OrganizerId == organizerId
                )
                .OrderBy(cityEvent => cityEvent.EventDate)
                .ThenBy(cityEvent => cityEvent.StartTime)
                .ToListAsync();
        }

        public async Task<bool> EventTypeExistsAsync(
            int eventTypeId)
        {
            return await _context.EventTypes
                .AsNoTracking()
                .AnyAsync(
                    eventType =>
                        eventType.EventTypeId == eventTypeId
                );
        }

        public async Task AddAsync(CityEvent cityEvent)
        {
            await _context.Events.AddAsync(cityEvent);
        }

        public void Delete(CityEvent cityEvent)
        {
            _context.Events.Remove(cityEvent);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<CityEvent>> SearchAsync(EventSearchDto searchDto)
        {
            IQueryable<CityEvent> query = _context.Events
                .AsNoTracking()
                .Include(cityEvent => cityEvent.EventType)
                .Include(cityEvent => cityEvent.Organizer)
                .Include(cityEvent => cityEvent.Registrations);

            if (!string.IsNullOrWhiteSpace(searchDto.Title))
            {
                string title = searchDto.Title.Trim();

                query = query.Where(cityEvent =>
                    EF.Functions.Like(
                        cityEvent.Title,
                        $"%{title}%"
                    )
                );
            }

            if (searchDto.Date.HasValue)
            {
                DateTime selectedDate =
                    searchDto.Date.Value.Date;

                DateTime nextDate =
                    selectedDate.AddDays(1);

                query = query.Where(cityEvent =>
                    cityEvent.EventDate >= selectedDate &&
                    cityEvent.EventDate < nextDate
                );
            }

            if (searchDto.EventTypeId.HasValue)
            {
                query = query.Where(cityEvent =>
                    cityEvent.EventTypeId ==
                    searchDto.EventTypeId.Value
                );
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Venue))
            {
                string venue = searchDto.Venue.Trim();

                query = query.Where(cityEvent =>
                    EF.Functions.Like(
                        cityEvent.Venue,
                        $"%{venue}%"
                    )
                );
            }

            if (!string.IsNullOrWhiteSpace(searchDto.Status))
            {
                string status = searchDto.Status.Trim();

                query = query.Where(cityEvent =>
                    cityEvent.Status == status
                );
            }

            return await query
                .OrderBy(cityEvent => cityEvent.EventDate)
                .ThenBy(cityEvent => cityEvent.StartTime)
                .ToListAsync();
        }
    }
}