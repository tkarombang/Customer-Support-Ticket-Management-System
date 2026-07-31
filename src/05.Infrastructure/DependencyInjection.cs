using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Services;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Infrastructure.DataProtection;
using TicketManagement.Infrastructure.Persistence;
using TicketManagement.Infrastructure.Persistence.Seed;
using TicketManagement.Infrastructure.Repositories;

namespace TicketManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection")));

        // Repository
        // --- Existing (v1, tetap dipakai) ---
        services.AddScoped<ITicketRepository, TicketRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IAuthService, AuthService>();

        // --- Baru (v2 — modul Users, Profile, Ticket Histories, dst) ---
        services.AddScoped<ITicketHistoryRepository, TicketHistoryRepository>();
        services.AddScoped<ISystemLogRepository, SystemLogRepository>();
        services.AddScoped<IAppSettingRepository, AppSettingRepository>();
        services.AddScoped<IIntegrationConfigRepository, IntegrationConfigRepository>();
        services.AddScoped<IBackupRepository, BackupRepository>();
        services.AddScoped<ITicketSequenceRepository, TicketSequenceRepository>();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<ISystemLogService, SystemLogService>();
        services.AddScoped<ITicketHistoryService, TicketHistoryService>();
        services.AddScoped<ISettingsService, SettingsService>();

        services.AddDataProtection(); // untuk CredentialEncryptor
        services.AddScoped<ICredentialEncryptor, CredentialEncryptor>();

        // --- JWT Authentication ---
        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"]!;

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        services.AddAuthorization();


        return services;
    }


    public static async Task InitialiseDatabaseAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        await context.Database.MigrateAsync();

        await DatabaseSeeder.SeedAsync(context);
    }
}