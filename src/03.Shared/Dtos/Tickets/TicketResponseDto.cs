namespace TicketManagement.Shared.Dtos.Tickets;

public class TicketResponseDto
{
    public Guid TicketId { get; set; }
    public required string TicketNumber { get; set; }
    public required string Type { get; set; }
    public required string Impact { get; set; }
    public required string Category { get; set; }
    public string? ApplicationSystem { get; set; }
    public required string Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string Status { get; set; }
    public Guid? AssignedToUserId { get; set; }
    public string? AssignedToAgentName { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<TicketAttachmentResponseDto> Attachments { get; set; } = [];
    public List<TicketCommentResponseDto> Comments { get; set; } = [];
}