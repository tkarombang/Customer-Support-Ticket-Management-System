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


    /// <summary>
    /// Cek apakah tiket sudah melewati batas waktu penyelesaian.
    /// Prioritas pengecekan: DueDate manual (jika diisi) > SLA otomatis dari Priority.
    /// Hanya relevan untuk tiket yang masih aktif (Open/InProgres).
    /// </summary>
    public bool IsOverdue(int slaHighHours, int slaMediumHours, int slaLowHours)
    {
        if (Status != TicketStatus.Open && Status != TicketStatus.InProgress)
            return false;

        if (DueDate.HasValue)
            return DateTime.UtcNow > DueDate.Value;

        var targetHours = Priority switch
        {
            TicketPriority.High => slaHighHours,
            TicketPriority.Medium => slaMediumHours,
            _ => slaLowHours
        };

        return (DateTime.UtcNow - CreatedDate).TotalHours > targetHours;
    }


}