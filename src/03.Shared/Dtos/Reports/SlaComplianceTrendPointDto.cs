using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Reports
{
    public class SlaComplianceTrendPointDto
    {
        public DateTime Date { get; set; }
        public double CompliancePercentage { get; set; }
    }
}
