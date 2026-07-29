using TicketManagement.Base.Common;

namespace TicketManagement.Domain.Entities;

public class TicketComment : BaseEntity
{
    public Guid TicketId { get; set; }
    public required string Content { get; set; }
    public Guid CreatedBy { get; set; }

    public Ticket? Ticket { get; set; }
    public User? CreatedByUser { get; set; }
}