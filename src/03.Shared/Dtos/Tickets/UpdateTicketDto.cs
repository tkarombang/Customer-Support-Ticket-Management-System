using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Shared.Dtos.Tickets;

public class UpdateTicketDto
{
    public required string Type { get; set; }
    public required string Impact { get; set; }
    public required string Category { get; set; }
    public string? ApplicationSystem { get; set; }
    public required string Priority { get; set; }
    public DateTime? DueDate { get; set; }

    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; } // "Open", "In Progress", "Resolved", "Closed"
    public string? StatusNote { get; set; } // catatan opsional saat ganti status (jadi TicketComment
}