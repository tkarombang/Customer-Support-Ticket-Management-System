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
                Id = Guid.Parse("B2C3D4E5-F6A7-4B8C-9D0E-1F2A3B4C5D6E"),
                Username = "admin",
                Name = "System Administrator",
                Email = "admin@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                Role = UserRole.Administrator,
                Status = UserStatus.Active,
                CreatedDate = new DateTime(2026, 1, 1)
            },

            new()
            {
                Id = Guid.Parse("7F3D9A2C-6B81-4E58-9F2A-3C7D8E1B5A94"),
                Username = "Viewer",
                Name = "Default Viewer",
                Email = "viewer@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Viewer123!"),
                Role = UserRole.Viewer,
                Status = UserStatus.Active,
                CreatedDate = new DateTime(2026,1,1)
            },

             new()
            {
                Id = Guid.Parse("C3D4E5F6-A7B8-4C9D-8E1F-2A3B4C5D6E7F"),
                Username = "agent1",
                Name = "Agent One",
                Email = "agent1@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.Agent,
                Status = UserStatus.Active,
                CreatedDate = new DateTime(2026, 1, 1)
            },

            new()
            {
                Id = Guid.Parse("D4E5F6A7-B8C9-4D1E-8F2A-3B4C5D6E7F80"),
                Username = "agent2",
                Name = "Agent Two",
                Email = "agent2@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.Agent,
                Status = UserStatus.Active,
                CreatedDate = new DateTime(2026, 1, 1)
            },

            new()
            {
                Id = Guid.Parse("E5F6A7B8-C9D0-4E2F-8A3B-4C5D6E7F8091"),
                Username = "agent3",
                Name = "Agent Three",
                Email = "agent3@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.Agent,
                Status = UserStatus.Active,
                CreatedDate = new DateTime(2026, 1, 1)
            },

            new()
            {
                Id = Guid.Parse("F6A7B8C9-D0E1-4F3A-8B4C-5D6E7F8091A2"),
                Username = "agent4",
                Name = "Agent Four",
                Email = "agent4@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.Agent,
                Status = UserStatus.Active,
                CreatedDate = new DateTime(2026, 1, 1)
            },

            new()
            {
                Id = Guid.Parse("A7B8C9D0-E1F2-4A4B-8C5D-6E7F8091A2B3"),
                Username = "agent5",
                Name = "Agent Five",
                Email = "agent5@ticket.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Agent123!"),
                Role = UserRole.Agent,
                Status = UserStatus.Active,
                CreatedDate = new DateTime(2026, 1, 1)
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