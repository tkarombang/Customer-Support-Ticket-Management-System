using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.TicketHistories
{
    public class TicketHistoryItemDto
    {
        public Guid HistoryId { get; set; }
        public required string TicketNumber { get; set; }
        public required string Action { get; set; }
        public string? PreviousStatus { get; set; }
        public string? NewStatus { get; set; }
        public required string ChangedByName { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
