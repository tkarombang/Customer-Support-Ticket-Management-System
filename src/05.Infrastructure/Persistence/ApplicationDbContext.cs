using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Default Manager",
                Email = "manager@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                Role = UserRole.Manager,
                CreatedDate = new DateTime(2026, 1, 1)
            },
            new User
            {
                Id = 2,
                Name = "Agent One",
                Email = "agent1@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.SupportAgent,
                CreatedDate = new DateTime(2026, 1, 1)
            }
        );
    }
}