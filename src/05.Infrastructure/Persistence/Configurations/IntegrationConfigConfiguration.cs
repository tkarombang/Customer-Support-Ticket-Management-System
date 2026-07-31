using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations
{
    public class IntegrationConfigConfiguration : IEntityTypeConfiguration<IntegrationConfig>
    {
        public void Configure(EntityTypeBuilder<IntegrationConfig> builder)
        {
            builder.ToTable("IntegrationConfigs");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).HasColumnName("IntegrationId");
            builder.Property(i => i.Name).HasMaxLength(100).IsRequired();
            builder.Property(i => i.WebhookUrl).HasMaxLength(500);
        }
    }
}
