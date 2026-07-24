using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Shared.Dtos.Tickets;

public class CreateTicketDto
{
    [Required, StringLength(100)]
    public required string CustomerName { get; set; }

    [Required, EmailAddress, StringLength(150)]
    public required string CustomerEmail { get; set; }

    [Required, StringLength(200)]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }
}