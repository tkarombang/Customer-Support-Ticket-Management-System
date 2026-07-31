using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Settings
{
    public class IntegrationResponseDto
    {
        public Guid IntegrationId { get; set; }
        public required string Name { get; set; }
        public string? WebhookUrl { get; set; }
        public bool HasApiKey { get; set; } // true/false saja, TIDAK pernah kirim API key asli balik ke client
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
