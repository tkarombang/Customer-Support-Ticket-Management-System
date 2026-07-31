using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations
{
    public class AppSettingConfiguration : IEntityTypeConfiguration<AppSetting>
    {
        public void Configure(EntityTypeBuilder<AppSetting> builder)
        {
            builder.ToTable("AppSettings");
            builder.HasKey(s => s.SettingKey); // PK string, bukan Guid

            builder.Property(s => s.SettingKey).HasMaxLength(100);
            builder.Property(s => s.SettingValue); // NVARCHAR(MAX), default

            builder.HasOne(s => s.UpdatedByUser).WithMany()
                .HasForeignKey(s => s.UpdatedBy).OnDelete(DeleteBehavior.SetNull);
        }
    }
}
