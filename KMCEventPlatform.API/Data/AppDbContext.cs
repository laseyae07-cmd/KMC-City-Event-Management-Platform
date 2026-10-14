using KMCEventPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

namespace KMCEventPlatform.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<CityEvent> Events { get; set; }
        public DbSet<Registration> Registrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set database table names.
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<EventType>().ToTable("EventTypes");
            modelBuilder.Entity<CityEvent>().ToTable("Events");
            modelBuilder.Entity<Registration>().ToTable("Registrations");

            // Every user must have a unique email address.
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            // Every event type name must be unique.
            modelBuilder.Entity<EventType>()
                .HasIndex(eventType => eventType.TypeName)
                .IsUnique();

            // Prevent a participant from registering twice
            // for the same event.
            modelBuilder.Entity<Registration>()
                .HasIndex(registration => new
                {
                    registration.EventId,
                    registration.ParticipantId
                })
                .IsUnique();

            // One organizer can create many events.
            modelBuilder.Entity<CityEvent>()
                .HasOne(cityEvent => cityEvent.Organizer)
                .WithMany(user => user.CreatedEvents)
                .HasForeignKey(cityEvent => cityEvent.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // One event type can contain many events.
            modelBuilder.Entity<CityEvent>()
                .HasOne(cityEvent => cityEvent.EventType)
                .WithMany(eventType => eventType.Events)
                .HasForeignKey(cityEvent => cityEvent.EventTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // One event can contain many registrations.
            modelBuilder.Entity<Registration>()
                .HasOne(registration => registration.Event)
                .WithMany(cityEvent => cityEvent.Registrations)
                .HasForeignKey(registration => registration.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // One participant can have many registrations.
            modelBuilder.Entity<Registration>()
                .HasOne(registration => registration.Participant)
                .WithMany(user => user.Registrations)
                .HasForeignKey(registration => registration.ParticipantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Initial event categories.
            modelBuilder.Entity<EventType>().HasData(
                new EventType
                {
                    EventTypeId = 1,
                    TypeName = "Cultural",
                    Description = "Cultural programmes and activities"
                },
                new EventType
                {
                    EventTypeId = 2,
                    TypeName = "Educational",
                    Description = "Educational programmes and workshops"
                },
                new EventType
                {
                    EventTypeId = 3,
                    TypeName = "Sports",
                    Description = "Sporting activities and competitions"
                },
                new EventType
                {
                    EventTypeId = 4,
                    TypeName = "Community",
                    Description = "Community programmes and public activities"
                },
                new EventType
                {
                    EventTypeId = 5,
                    TypeName = "Religious",
                    Description = "Religious events and programmes"
                },
                new EventType
                {
                    EventTypeId = 6,
                    TypeName = "Entertainment",
                    Description = "Entertainment programmes and performances"
                },
                new EventType
                {
                    EventTypeId = 7,
                    TypeName = "Business",
                    Description = "Business events, exhibitions and conferences"
                }
            );
        }
    }
}