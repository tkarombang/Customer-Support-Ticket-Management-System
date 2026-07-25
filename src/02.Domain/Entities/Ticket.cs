using TicketManagement.Base.Common;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Domain.Entities;

public class Ticket : BaseEntity
{
    public required string TicketNumber { get; set; }

    public required string CustomerName { get; set; }

    public required string CustomerEmail { get; set; }

    public required string Title { get; set; }

    public required string Description { get; set; }

    public TicketStatus Status { get; set; } = TicketStatus.Open;

    public Guid? AssignedTo { get; set; }

    // Navigation properties
    public User? AssignedAgent { get; set; }

    public ICollection<TicketHistory> Histories { get; set; } = [];

    /// <summary>
    /// Business rule (REQ-2.5): tiket berstatus Closed tidak boleh dimodifikasi.
    /// Dicek di sini (domain-level) agar aturan tidak bisa "terlewat"
    /// walau dipanggil dari Service manapun.
    /// </summary>
    public bool IsModifiable() => Status != TicketStatus.Closed;
}