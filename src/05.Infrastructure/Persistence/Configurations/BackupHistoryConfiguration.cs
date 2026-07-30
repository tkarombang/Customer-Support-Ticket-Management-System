using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations
{
    public class BackupHistoryConfiguration : IEntityTypeConfiguration<BackupHistory>
    {
        public void Configure(EntityTypeBuilder<BackupHistory> builder)
        {
            builder.ToTable("BackupHistories");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).HasColumnName("BackupId");
            builder.Property(b => b.FileName).HasMaxLength(255).IsRequired();
            builder.Property(b => b.FilePath).HasMaxLength(500).IsRequired();
            builder.Property(b => b.Type).HasMaxLength(20).IsRequired();
            builder.Property(b => b.Status).HasMaxLength(20).IsRequired();

            builder.HasOne(b => b.TriggeredByUser).WithMany()
                .HasForeignKey(b => b.TriggeredBy).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
