using Microsoft.EntityFrameworkCore;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketHistory> TicketHistories => Set<TicketHistory>();
    public DbSet<TicketAttachment> TicketAttachments => Set<TicketAttachment>();
    public DbSet<TicketComment> TicketComments => Set<TicketComment>();
    public DbSet<TicketCc> TicketCcs => Set<TicketCc>();
    public DbSet<SystemLog> SystemLogs => Set<SystemLog>();
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();
    public DbSet<IntegrationConfig> IntegrationConfigs => Set<IntegrationConfig>();
    public DbSet<BackupHistory> BackupHistories => Set<BackupHistory>();
    public DbSet<TicketSequence> TicketSequences => Set<TicketSequence>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}