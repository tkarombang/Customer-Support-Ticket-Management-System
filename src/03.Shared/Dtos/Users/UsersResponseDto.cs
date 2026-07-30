using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Users
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public required string Username { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
