using KMCEventPlatform.API.Data;
using KMCEventPlatform.API.Models;
using KMCEventPlatform.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KMCEventPlatform.Tests
{
    public class RegistrationRepositoryTests
    {
        private static AppDbContext CreateContext()
        {
            var options =
                new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase(
                        Guid.NewGuid().ToString()
                    )
                    .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CountActiveRegistrationsAsync_CountsOnlyRegistered()
        {
            await using var context = CreateContext();

            context.Registrations.AddRange(
                new Registration
                {
                    RegistrationId = 1,
                    EventId = 1,
                    ParticipantId = 1,
                    Status = "Registered"
                },

                new Registration
                {
                    RegistrationId = 2,
                    EventId = 1,
                    ParticipantId = 2,
                    Status = "Registered"
                },

                new Registration
                {
                    RegistrationId = 3,
                    EventId = 1,
                    ParticipantId = 3,
                    Status = "Cancelled"
                }
            );

            await context.SaveChangesAsync();

            var repository =
                new RegistrationRepository(context);

            int result =
                await repository
                    .CountActiveRegistrationsAsync(1);

            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetByEventAndParticipantAsync_ReturnsCorrectRegistration()
        {
            await using var context = CreateContext();

            // Organizer
            context.Users.Add(
                new User
                {
                    UserId = 1,
                    FullName = "Test Organizer",
                    Email = "organizer@test.com",
                    PasswordHash = "TestHash",
                    Role = "Organizer"
                }
            );

            // Participant
            context.Users.Add(
                new User
                {
                    UserId = 5,
                    FullName = "Test Participant",
                    Email = "participant@test.com",
                    PasswordHash = "TestHash",
                    Role = "Participant"
                }
            );

            // Event type
            context.EventTypes.Add(
                new EventType
                {
                    EventTypeId = 1,
                    TypeName = "Cultural",
                    Description = "Cultural Events"
                }
            );

            // Event
            context.Events.Add(
                new CityEvent
                {
                    EventId = 10,
                    Title = "Test Event",
                    Description = "Test event description",
                    EventDate = DateTime.Today.AddDays(5),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Venue = "Kandy",
                    MaximumParticipants = 100,
                    Status = "Upcoming",
                    EventTypeId = 1,
                    OrganizerId = 1
                }
            );

            await context.SaveChangesAsync();

            // Registration
            context.Registrations.Add(
                new Registration
                {
                    RegistrationId = 1,
                    EventId = 10,
                    ParticipantId = 5,
                    Status = "Registered"
                }
            );

            await context.SaveChangesAsync();

            var repository =
                new RegistrationRepository(context);

            var result =
                await repository
                    .GetByEventAndParticipantAsync(
                        10,
                        5
                    );

            Assert.NotNull(result);

            Assert.Equal(
                "Registered",
                result.Status
            );

            Assert.Equal(
                "Test Participant",
                result.Participant?.FullName
            );

            Assert.Equal(
                "Test Event",
                result.Event?.Title
            );
        }
    }
}