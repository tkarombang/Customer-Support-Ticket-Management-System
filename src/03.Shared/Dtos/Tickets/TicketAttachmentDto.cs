using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Tickets
{
    public class TicketAttachmentResponseDto
    {
        public Guid AttachmentId { get; set; }
        public required string FileName { get; set; }
        public required string FilePath { get; set; }
        public long FileSizeBytes { get; set; }
        public required string UploadedByName { get; set; }
        public DateTime UploadedDate { get; set; }
    }
}
