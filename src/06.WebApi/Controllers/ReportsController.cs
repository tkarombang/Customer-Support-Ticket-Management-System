using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Application.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Reports;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.Reports.Base)]
[Authorize(Roles = RoleConstants.Administrator)]
public class ReportsController(IReportService reportService) : ControllerBase
{

    [HttpGet(ApiRoutes.Reports.ManagerReport)]
    public async Task<IActionResult> GetManagerReport(
    [FromQuery] ManagerReportFilterDto filter)
    {
        var result = await reportService.GetManagerReportAsync(filter);
        return Ok(result);
    }

    [HttpGet(ApiRoutes.Reports.SlaCompliance)]
    public async Task<IActionResult> GetSlaCompliannceAsync(DateTime? startDate, DateTime? endDate)
    {
        var result = await reportService.GetSlaCompliannceAsync(startDate, endDate);
        return Ok(result);
    }

    [HttpGet(ApiRoutes.Reports.ResponseTime)]
    public async Task<IActionResult> GetResponseTime(DateTime? startDate, DateTime? endDate)
    {
        var result = await reportService.GetResponseTimeAsync(startDate, endDate);
        return Ok(result);
    }
}