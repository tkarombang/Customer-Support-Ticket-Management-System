using TicketManagement.Base.Common;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Domain.Entities;

public class TicketHistory : BaseEntity
{
    public Guid TicketId { get; set; }

    public required string Action { get; set; } // "Created", "Assigned", "StatusChanged"

    public TicketStatus? PreviousStatus { get; set; }

    public TicketStatus? NewStatus { get; set; }

    public Guid ChangedBy { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Ticket? Ticket { get; set; }

    public User? ChangedByUser { get; set; }
}