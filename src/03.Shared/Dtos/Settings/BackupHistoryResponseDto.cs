using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Settings
{
    public class BackupHistoryResponseDto
    {
        public Guid BackupId { get; set; }
        public required string FileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public required string Type { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? TriggeredByName { get; set; }
    }
}
