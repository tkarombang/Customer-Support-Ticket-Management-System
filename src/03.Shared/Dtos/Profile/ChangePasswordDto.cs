using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Profile
{
    public class ChangePasswordDto
    {
        public required string OldPassword { get; set; }
        public required string NewPassword { get; set; }
    }
}
