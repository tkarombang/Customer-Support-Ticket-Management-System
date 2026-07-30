using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Profile
{
    public class UpdateProfileDto
    {
        public required string Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
    }
}
