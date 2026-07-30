using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Users
{
    public class CreateUserDto
    {
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; } // "Administrator" | "Agent" | "Viewer"
    }
}
