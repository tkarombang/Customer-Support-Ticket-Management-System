using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Tickets
{
    public class CreateCommentDto
    {
        public required string Content { get; set; }
    }
}
