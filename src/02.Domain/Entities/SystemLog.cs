using TicketManagement.Base.Common;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Domain.Entities;

public class SystemLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public SystemLogAction Action { get; set; }
    public required string Description { get; set; }
    public string? IpAddress { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public User? User { get; set; }
}