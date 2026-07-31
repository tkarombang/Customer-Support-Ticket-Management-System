using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Infrastructure.Persistence.Configurations
{
    public class TicketCcConfiguration : IEntityTypeConfiguration<TicketCc>
    {
        public void Configure(EntityTypeBuilder<TicketCc> builder)
        {
            builder.ToTable("TicketCc");
            builder.HasKey(cc => new { cc.TicketId, cc.UserId }); // composite key

            builder.HasOne(cc => cc.Ticket).WithMany(t => t.CcUsers)
                .HasForeignKey(cc => cc.TicketId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(cc => cc.User).WithMany()
                .HasForeignKey(cc => cc.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
