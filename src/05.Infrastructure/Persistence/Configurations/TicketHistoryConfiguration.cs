using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations;

public class TicketHistoryConfiguration : IEntityTypeConfiguration<TicketHistory>
{
    public void Configure(EntityTypeBuilder<TicketHistory> builder)
    {
        builder.ToTable("TicketHistories");
        builder.HasKey(h => h.Id);
        builder.Property(h => h.Id).HasColumnName("HistoryId");
        builder.Property(h => h.Action).HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(h => h.PreviousStatus).HasConversion<string>().HasMaxLength(20);
        builder.Property(h => h.NewStatus).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(h => h.Ticket).WithMany(t => t.Histories)
            .HasForeignKey(h => h.TicketId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(h => h.ChangedByUser).WithMany()
            .HasForeignKey(h => h.ChangedBy).OnDelete(DeleteBehavior.Restrict);
    }
}