using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Infrastructure.Persistence.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var users = new List<User>
        {
            new()
            {
                Id = Guid.Parse("A1B2C3D4-E5F6-4A5B-8C9D-0E1F2A3B4C5D"),
                Name = "Default Manager",
                Email = "manager@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                Role = UserRole.Manager,
                CreatedDate = new DateTime(2026,1,1)
            },

            new()
            {
                Id = Guid.Parse("C3D4E5F6-A7B8-4C9D-8E1F-2A3B4C5D6E7F"),
                Name = "Agent One",
                Email = "agent1@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.SupportAgent,
                CreatedDate = new DateTime(2026,1,1)
            }
        };

        await context.Users.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}