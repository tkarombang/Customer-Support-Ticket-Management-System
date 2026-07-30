using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Profile
{
    public class ActivityLogDto
    {
        public required string Action { get; set; }
        public required string Description { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
