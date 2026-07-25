using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Shared.Dtos.Tickets;

public class AssignTicketDto
{
    [Required]
    public Guid AssignedToUserId { get; set; }
}