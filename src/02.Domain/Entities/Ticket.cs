using TicketManagement.Base.Common;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Domain.Entities;

public class Ticket : BaseEntity
{
    public required string TicketNumber { get; set; } // tetap string, TKT-00001

    public TicketType Type { get; set; } = TicketType.Incident;
    public TicketImpact Impact { get; set; }
    public TicketCategory Category { get; set; }
    public string? ApplicationSystem { get; set; }
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    public DateTime? DueDate { get; set; }

    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public Guid? AssignedTo { get; set; } // FK -> User.Id (Guid)

    public User? AssignedAgent { get; set; }
    public ICollection<TicketHistory> Histories { get; set; } = [];
    public ICollection<TicketAttachment> Attachments { get; set; } = [];
    public ICollection<TicketComment> Comments { get; set; } = [];
    public ICollection<TicketCc> CcUsers { get; set; } = [];

    public bool IsModifiable() => Status != TicketStatus.Closed;
}