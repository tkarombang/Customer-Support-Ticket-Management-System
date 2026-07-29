using TicketManagement.Base.Common;

namespace TicketManagement.Domain.Entities;

public class TicketAttachment : BaseEntity
{
    public Guid TicketId { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public long FileSizeBytes { get; set; }
    public required string ContentType { get; set; }
    public Guid UploadedBy { get; set; }

    public Ticket? Ticket { get; set; }
    public User? UploadedByUser { get; set; }
}