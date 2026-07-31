using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Settings
{
    public class CreateIntegrationDto
    {
        public required string Name { get; set; }
        public string? WebhookUrl { get; set; }
        public string? ApiKey { get; set; } // plaintext saat input, dienkripsi sebelum disimpan
    }
}
