using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Infrastructure.Persistence.Seed;

public static class UserSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {

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
            },
            new()
            {
                Id = Guid.Parse("D4E5F6A7-B8C9-4D1E-8F2A-3B4C5D6E7F80"),
                Name = "Agent Two",
                Email = "agent2@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.SupportAgent,
                CreatedDate = new DateTime(2026,07,27)
            },
            new()
            {
                Id = Guid.Parse("E5F6A7B8-C9D0-4E2F-8A3B-4C5D6E7F8091"),
                Name = "Agent Three",
                Email = "agent3@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.SupportAgent,
                CreatedDate = new DateTime(2026,07,27)
            },
            new()
            {
                Id = Guid.Parse("F6A7B8C9-D0E1-4F3A-8B4C-5D6E7F8091A2"),
                Name = "Agent Four",
                Email = "agent4@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.SupportAgent,
                CreatedDate = new DateTime(2026,07,27)
            },
            new()
            {
                Id = Guid.Parse("A7B8C9D0-E1F2-4A4B-8C5D-6E7F8091A2B3"),
                Name = "Agent Five",
                Email = "agent5@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.SupportAgent,
                CreatedDate = new DateTime(2026,07,27)
            }

        };

        foreach (var user in users)
        {
            if (!await context.Users.AnyAsync(x => x.Email == user.Email))
            {
                context.Users.Add(user);
            }
        }

        await context.SaveChangesAsync();
    }
}