using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Shared.Dtos.Tickets;

public class AssignTicketDto
{
    [Required]
    public int AssignedToUserId { get; set; }
}