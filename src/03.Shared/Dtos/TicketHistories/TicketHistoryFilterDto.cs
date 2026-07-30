using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.TicketHistories
{
    public class TicketHistoryFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Action { get; set; }
        public Guid? UserId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
