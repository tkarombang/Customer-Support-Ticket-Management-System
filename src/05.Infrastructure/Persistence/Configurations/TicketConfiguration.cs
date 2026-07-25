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
        builder.HasIndex(t => t.TicketNumber).IsUnique(); // REQ-2.2

        builder.Property(t => t.CustomerName).HasMaxLength(100).IsRequired();
        builder.Property(t => t.CustomerEmail).HasMaxLength(150).IsRequired();
        builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Description).IsRequired(); // NVARCHAR(MAX) default

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(Domain.Enums.TicketStatus.Open) // REQ-2.3
            .IsRequired();

        // Composite index untuk optimasi Manager Report query (tech.md Section 4.2)
        builder.HasIndex(t => new { t.CreatedDate, t.Status, t.AssignedTo });

        builder.HasOne(t => t.AssignedAgent)
            .WithMany(u => u.AssignedTickets)
            .HasForeignKey(t => t.AssignedTo)
            .OnDelete(DeleteBehavior.SetNull); // agent dihapus -> ticket jadi unassigned, bukan ikut terhapus
    }
}