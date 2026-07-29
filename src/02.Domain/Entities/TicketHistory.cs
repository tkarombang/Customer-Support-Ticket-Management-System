using TicketManagement.Base.Common;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Domain.Entities;

public class TicketHistory : BaseEntity
{
    public Guid TicketId { get; set; }
    public HistoryAction Action { get; set; }
    public TicketStatus? PreviousStatus { get; set; }
    public TicketStatus? NewStatus { get; set; }
    public Guid ChangedBy { get; set; } // FK -> User.Id

    public Ticket? Ticket { get; set; }
    public User? ChangedByUser { get; set; }
}