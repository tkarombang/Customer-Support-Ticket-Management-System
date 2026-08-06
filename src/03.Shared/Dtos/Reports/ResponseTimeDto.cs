using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Reports
{
    public class ResponseTimeDto
    {
        public double AverageResponseHours { get; set; }
        public double AverageResponseHoursPreviousPeriod { get; set; } // untuk hitung % perubahan
    }
}
