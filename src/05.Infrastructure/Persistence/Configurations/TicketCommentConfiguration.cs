using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations
{
    public class TicketCommentConfiguration : IEntityTypeConfiguration<TicketComment>
    {
        public void Configure(EntityTypeBuilder<TicketComment> builder)
        {
            builder.ToTable("TicketComments");
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("CommentId");
            builder.Property(c => c.Content).HasMaxLength(1000).IsRequired();

            builder.HasOne(c => c.Ticket).WithMany(t => t.Comments)
                .HasForeignKey(c => c.TicketId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(c => c.CreatedByUser).WithMany()
                .HasForeignKey(c => c.CreatedBy).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
