using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Settings
{
    public class UpdateIntegrationDto
    {
        public required string Name { get; set; }
        public string? WebhookUrl { get; set; }
        public string? ApiKey { get; set; } // kosong = tidak diubah
        public bool IsActive { get; set; }
    }
}
