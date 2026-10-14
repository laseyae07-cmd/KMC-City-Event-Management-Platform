using KMCEventPlatform.API.Data;
using KMCEventPlatform.API.Models;
using KMCEventPlatform.API.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace KMCEventPlatform.Tests
{
    public class EventRepositoryTests
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

        private static async Task AddRequiredData(
            AppDbContext context)
        {
            context.Users.AddRange(
                new User
                {
                    UserId = 1,
                    FullName = "Organizer One",
                    Email = "organizer1@test.com",
                    PasswordHash = "TestHash",
                    Role = "Organizer"
                },
                new User
                {
                    UserId = 2,
                    FullName = "Organizer Two",
                    Email = "organizer2@test.com",
                    PasswordHash = "TestHash",
                    Role = "Organizer"
                }
            );

            context.EventTypes.AddRange(
                new EventType
                {
                    EventTypeId = 1,
                    TypeName = "Cultural",
                    Description = "Cultural Events"
                },
                new EventType
                {
                    EventTypeId = 2,
                    TypeName = "Educational",
                    Description = "Educational Events"
                }
            );

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEvents()
        {
            await using var context = CreateContext();

            await AddRequiredData(context);

            context.Events.AddRange(
                new CityEvent
                {
                    EventId = 1,
                    Title = "First Test Event",
                    Description = "Test Description",
                    EventDate = DateTime.Today.AddDays(5),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Venue = "Kandy",
                    MaximumParticipants = 100,
                    Status = "Upcoming",
                    EventTypeId = 1,
                    OrganizerId = 1
                },

                new CityEvent
                {
                    EventId = 2,
                    Title = "Second Test Event",
                    Description = "Test Description",
                    EventDate = DateTime.Today.AddDays(10),
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(14, 0, 0),
                    Venue = "Peradeniya",
                    MaximumParticipants = 200,
                    Status = "Upcoming",
                    EventTypeId = 2,
                    OrganizerId = 1
                }
            );

            await context.SaveChangesAsync();

            var repository =
                new EventRepository(context);

            var result =
                await repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectEvent()
        {
            await using var context = CreateContext();

            await AddRequiredData(context);

            context.Events.Add(
                new CityEvent
                {
                    EventId = 1,
                    Title = "Kandy Festival",
                    Description = "Festival Description",
                    EventDate = DateTime.Today.AddDays(5),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(17, 0, 0),
                    Venue = "Kandy",
                    MaximumParticipants = 500,
                    Status = "Upcoming",
                    EventTypeId = 1,
                    OrganizerId = 1
                }
            );

            await context.SaveChangesAsync();

            var repository =
                new EventRepository(context);

            var result =
                await repository.GetByIdAsync(1);

            Assert.NotNull(result);

            Assert.Equal(
                "Kandy Festival",
                result.Title
            );
        }

        [Fact]
        public async Task GetByOrganizerIdAsync_ReturnsOnlyOrganizerEvents()
        {
            await using var context = CreateContext();

            await AddRequiredData(context);

            context.Events.AddRange(
                new CityEvent
                {
                    EventId = 1,
                    Title = "Organizer One Event",
                    Description = "Description",
                    EventDate = DateTime.Today.AddDays(5),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Venue = "Kandy",
                    MaximumParticipants = 100,
                    Status = "Upcoming",
                    EventTypeId = 1,
                    OrganizerId = 1
                },

                new CityEvent
                {
                    EventId = 2,
                    Title = "Organizer Two Event",
                    Description = "Description",
                    EventDate = DateTime.Today.AddDays(6),
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(12, 0, 0),
                    Venue = "Kandy",
                    MaximumParticipants = 100,
                    Status = "Upcoming",
                    EventTypeId = 1,
                    OrganizerId = 2
                }
            );

            await context.SaveChangesAsync();

            var repository =
                new EventRepository(context);

            var result =
                await repository
                    .GetByOrganizerIdAsync(1);

            Assert.Single(result);

            Assert.Equal(
                "Organizer One Event",
                result.First().Title
            );
        }
    }
}