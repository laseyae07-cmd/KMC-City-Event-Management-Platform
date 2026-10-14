using KMCEventPlatform.API.Data;
using KMCEventPlatform.API.Interfaces;
using KMCEventPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

namespace KMCEventPlatform.API.Repositories
{
    public class RegistrationRepository
        : IRegistrationRepository
    {
        private readonly AppDbContext _context;

        public RegistrationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Registration?> GetByIdAsync(
            int registrationId,
            bool trackChanges = false)
        {
            IQueryable<Registration> query =
                _context.Registrations
                    .Include(registration => registration.Event)
                    .Include(registration => registration.Participant);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(
                registration =>
                    registration.RegistrationId == registrationId
            );
        }

        public async Task<Registration?>
            GetByEventAndParticipantAsync(
                int eventId,
                int participantId,
                bool trackChanges = false)
        {
            IQueryable<Registration> query =
                _context.Registrations
                    .Include(registration => registration.Event)
                    .Include(registration => registration.Participant);

            if (!trackChanges)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync(
                registration =>
                    registration.EventId == eventId &&
                    registration.ParticipantId == participantId
            );
        }

        public async Task<IEnumerable<Registration>>
            GetByParticipantIdAsync(int participantId)
        {
            return await _context.Registrations
                .AsNoTracking()
                .Include(registration => registration.Event)
                .Include(registration => registration.Participant)
                .Where(
                    registration =>
                        registration.ParticipantId == participantId
                )
                .OrderBy(
                    registration =>
                        registration.Event!.EventDate
                )
                .ToListAsync();
        }

        public async Task<IEnumerable<Registration>>
            GetByEventIdAsync(int eventId)
        {
            return await _context.Registrations
                .AsNoTracking()
                .Include(registration => registration.Event)
                .Include(registration => registration.Participant)
                .Where(
                    registration =>
                        registration.EventId == eventId
                )
                .OrderBy(
                    registration =>
                        registration.Participant!.FullName
                )
                .ToListAsync();
        }

        public async Task<int> CountActiveRegistrationsAsync(
            int eventId)
        {
            return await _context.Registrations.CountAsync(
                registration =>
                    registration.EventId == eventId &&
                    registration.Status == "Registered"
            );
        }

        public async Task AddAsync(
            Registration registration)
        {
            await _context.Registrations.AddAsync(registration);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}