using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.ToTable("Tickets");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("TicketId");

        builder.Property(t => t.TicketNumber).HasMaxLength(20).IsRequired();
        builder.HasIndex(t => t.TicketNumber).IsUnique();

        builder.Property(t => t.Type).HasConversion<string>().HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.TicketType.Incident).IsRequired();
        builder.Property(t => t.Impact).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(t => t.Category).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(t => t.ApplicationSystem).HasMaxLength(100);
        builder.Property(t => t.Priority).HasConversion<string>().HasMaxLength(10)
            .HasDefaultValue(Domain.Enums.TicketPriority.Medium).IsRequired();

        builder.Property(t => t.CustomerName).HasMaxLength(100).IsRequired();
        builder.Property(t => t.CustomerEmail).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).IsRequired();

        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.TicketStatus.Open).IsRequired();

        builder.HasIndex(t => new { t.CreatedDate, t.Status, t.AssignedTo });

        builder.HasOne(t => t.AssignedAgent)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedTo)
            .OnDelete(DeleteBehavior.SetNull);
    }
}