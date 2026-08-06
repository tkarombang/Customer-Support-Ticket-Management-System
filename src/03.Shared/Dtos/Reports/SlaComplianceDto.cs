using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.Shared.Dtos.Reports
{
    public class SlaComplianceDto
    {
        public double CompliancePercentage { get; set; }
        public int TotalResolved { get; set; }
        public int WithinSla { get; set; }
        public int BreachedSla { get; set; }
        public List<SlaComplianceTrendPointDto> Trend { get; set; } = [];
    }
}
