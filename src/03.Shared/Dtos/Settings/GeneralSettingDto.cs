using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Settings
{
    public class GeneralSettingDto
    {
        public string AppName { get; set; } = "Ticket System";
        public string? AppDescription { get; set; }
        public string TimeZone { get; set; } = "Asia/Jakarta";
        public string DateFormat { get; set; } = "dd MMM yyyy";
        public string TimeFormat { get; set; } = "24h";
        public string Language { get; set; } = "id";
        public string Currency { get; set; } = "IDR";
        public int ItemsPerPage { get; set; } = 10;
    }
}
