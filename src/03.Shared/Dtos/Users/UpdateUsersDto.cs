using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Users
{
    public class UpdateUserDto
    {
        public required string Name { get; set; }
        public required string Role { get; set; }
        public string? PhoneNumber { get; set; }
        public string? JobTitle { get; set; }
        public string? Address { get; set; }
    }
}
