using TicketManagement.Shared.Dtos.Reports;
using TicketManagement.Shared.Models;

namespace TicketManagement.Application.Interfaces;

public interface IReportService
{
    Task<PagedResult<ManagerReportItemDto>> GetManagerReportAsync(ManagerReportFilterDto filter);
    Task<DashboardSummaryDto> GetDashboardSummaryAsync();
}