using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations
{
    public class SystemLogConfiguration : IEntityTypeConfiguration<SystemLog>
    {
        public void Configure(EntityTypeBuilder<SystemLog> builder)
        {
            builder.ToTable("SystemLogs");
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).HasColumnName("LogId");
            builder.Property(l => l.Action).HasConversion<string>().HasMaxLength(50).IsRequired();
            builder.Property(l => l.Description).HasMaxLength(500).IsRequired();
            builder.Property(l => l.IpAddress).HasMaxLength(45);

            builder.HasIndex(l => l.Timestamp);
            builder.HasIndex(l => l.UserId);

            builder.HasOne(l => l.User).WithMany()
                .HasForeignKey(l => l.UserId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
