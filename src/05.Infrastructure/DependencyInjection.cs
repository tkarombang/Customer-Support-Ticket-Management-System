using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketManagement.Infrastructure.Persistence;

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

        //// Repository
        //services.AddScoped<IUserRepository, UserRepository>();
        //services.AddScoped<ITicketRepository, TicketRepository>();
        //services.AddScoped<IReportRepository, ReportRepository>();

        return services;
    }
}