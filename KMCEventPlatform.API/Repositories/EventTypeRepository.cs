using KMCEventPlatform.API.Data;
using KMCEventPlatform.API.Interfaces;
using KMCEventPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

namespace KMCEventPlatform.API.Repositories
{
    public class EventTypeRepository : IEventTypeRepository
    {
        private readonly AppDbContext _context;

        public EventTypeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventType>> GetAllAsync()
        {
            return await _context.EventTypes
                .AsNoTracking()
                .OrderBy(eventType => eventType.TypeName)
                .ToListAsync();
        }

        public async Task<EventType?> GetByIdAsync(int id)
        {
            return await _context.EventTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    eventType => eventType.EventTypeId == id
                );
        }
    }
}