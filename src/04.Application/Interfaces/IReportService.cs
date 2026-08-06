using TicketManagement.Shared.Dtos.Reports;
using TicketManagement.Shared.Models;

namespace TicketManagement.Application.Interfaces;

public interface IReportService
{
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
    Task<PagedResult<ManagerReportItemDto>> GetManagerReportAsync(ManagerReportFilterDto filter);
    Task<SlaComplianceDto> GetSlaCompliannceAsync(DateTime? startDate, DateTime? endDate);
    Task<ResponseTimeDto> GetResponseTimeAsync(DateTime? startDate, DateTime? endDate);
}