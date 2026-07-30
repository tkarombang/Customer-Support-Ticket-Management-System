using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Tickets
{
    public class TicketCommentResponseDto
    {
        public Guid CommentId { get; set; }
        public required string Content { get; set; }
        public required string CreatedByName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
