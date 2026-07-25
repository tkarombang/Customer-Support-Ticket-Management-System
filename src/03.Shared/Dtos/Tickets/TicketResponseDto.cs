namespace TicketManagement.Shared.Dtos.Tickets;

public class TicketResponseDto
{
    public Guid TicketId { get; set; }
    public required string TicketNumber { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToAgentName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}