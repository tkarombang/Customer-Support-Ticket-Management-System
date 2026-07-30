using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Shared.Dtos.Tickets;

public class CreateTicketDto
{
    [Required]
    public required string Type { get; set; } // "Incident" | "Request" | "Problem"

    [Required]
    public required string Impact { get; set; }

    [Required]
    public required string Category { get; set; }

    public string? ApplicationSystem { get; set; }

    [Required]
    public required string Priority { get; set; } // "Low" | "Medium" | "High"

    public DateTime? DueDate { get; set; }

    [Required, StringLength(100)]
    public required string CustomerName { get; set; }

    [Required, EmailAddress, StringLength(150)]
    public required string CustomerEmail { get; set; }

    [Required, StringLength(200)]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }

    public Guid? AssignedToUserId { get; set; } // opsional saat create, sesuai mockup "Assign To"

    public List<Guid>? CcUserIds { get; set; } // REQ-2.14
}