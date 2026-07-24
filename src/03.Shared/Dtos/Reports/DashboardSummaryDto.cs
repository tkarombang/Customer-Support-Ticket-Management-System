namespace TicketManagement.Shared.Dtos.Reports;

public class DashboardSummaryDto
{
    public int TotalTickets { get; set; }
    public int OpenCount { get; set; }
    public int InProgressCount { get; set; }
    public int ResolvedCount { get; set; }
    public int ClosedCount { get; set; }
    public List<AgentWorkloadDto> WorkloadPerAgent { get; set; } = [];
}

public class AgentWorkloadDto
{
    public int UserId { get; set; }
    public required string AgentName { get; set; }
    public int AssignedTicketCount { get; set; }
}