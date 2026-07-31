using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations
{
    public class TicketAttachmentConfiguration : IEntityTypeConfiguration<TicketAttachment>
    {
        public void Configure(EntityTypeBuilder<TicketAttachment> builder)
        {
            builder.ToTable("TicketAttachments");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("AttachmentId");
            builder.Property(a => a.FileName).HasMaxLength(255).IsRequired();
            builder.Property(a => a.FilePath).HasMaxLength(500).IsRequired();
            builder.Property(a => a.ContentType).HasMaxLength(100).IsRequired();

            builder.HasOne(a => a.Ticket).WithMany(t => t.Attachments)
                .HasForeignKey(a => a.TicketId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(a => a.UploadedByUser).WithMany()
                .HasForeignKey(a => a.UploadedBy).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
