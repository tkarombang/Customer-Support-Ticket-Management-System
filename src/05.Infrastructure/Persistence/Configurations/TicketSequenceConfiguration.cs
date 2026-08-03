using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations
{
    public class TicketSequenceConfiguration : IEntityTypeConfiguration<TicketSequence>
    {
        public void Configure(EntityTypeBuilder<TicketSequence> builder)
        {
            builder.ToTable("TicketSequences");
            builder.HasKey(s => s.Id);
            builder.HasIndex(s => s.Year).IsUnique();
        }
    }
}
