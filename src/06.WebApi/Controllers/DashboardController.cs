using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Application.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Reports;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.Reports.Base)]
[Authorize(Roles = RoleConstants.Manager)]
public class DashboardController(IReportService reportService) : ControllerBase
{
    [HttpGet(ApiRoutes.Reports.ManagerSegment)]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await reportService.GetDashboardSummaryAsync();
        return Ok(summary);
    }

    [HttpGet(ApiRoutes.Reports.ManagerReport)]
    public async Task<IActionResult> GetManagerReport(
    [FromQuery] ManagerReportFilterDto filter)
    {
        var result = await reportService.GetManagerReportAsync(filter);
        return Ok(result);
    }
}