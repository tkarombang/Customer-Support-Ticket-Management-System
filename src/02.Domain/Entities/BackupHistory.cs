using TicketManagement.Base.Common;

namespace TicketManagement.Domain.Entities;

public class BackupHistory : BaseEntity
{
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public long? FileSizeBytes { get; set; }
    public required string Type { get; set; }   // "Manual" / "Scheduled"
    public required string Status { get; set; } // "Success" / "Failed"
    public Guid? TriggeredBy { get; set; }

    public User? TriggeredByUser { get; set; }
}