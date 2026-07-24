using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Shared.Dtos.Tickets;

public class UpdateTicketDto
{
    [Required]
    public required string Description { get; set; }

    [Required]
    public required string Status { get; set; } // "Open", "In Progress", "Resolved", "Closed"
}