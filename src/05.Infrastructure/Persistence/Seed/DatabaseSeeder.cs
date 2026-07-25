using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Infrastructure.Persistence.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await UserSeeder.SeedAsync(context);
    }
}