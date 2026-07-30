using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Application.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Reports;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.Dashboard.Base)]
[Authorize(Roles = RoleConstants.Administrator)]
public class DashboardController(IReportService reportService) : ControllerBase
{
    [HttpGet(ApiRoutes.Dashboard.summary)]
    public async Task<IActionResult> GetSummary()
    {
        var summary = await reportService.GetDashboardSummaryAsync();
        return Ok(summary);
    }
}