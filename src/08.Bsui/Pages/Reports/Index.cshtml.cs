using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Reports;

namespace TicketManagement.Bsui.Pages.Reports;
public class IndexModel(ITicketApiClient apiClient) : PageModel
{
    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("Role") != "Administrator")
            return RedirectToPage("/Login");
        return Page();
    }

    // AJAX: GET /Reports?handler=ManagerReport
    public async Task<JsonResult> OnGetManagerReportAsync([FromQuery] ManagerReportFilterDto filter)
    {
        var token = HttpContext.Session.GetString("Token");
        var result = await apiClient.GetManagerReportAsync(filter, token!);
        return new JsonResult(result);
    }

    // AJAX: GET /Reports?handler=Sla
    public async Task<JsonResult> OnGetSlaAsync(DateTime? startDate, DateTime? endDate)
    {
        var token = HttpContext.Session.GetString("Token");
        var result = await apiClient.GetSlaComplianceAsync(startDate, endDate, token!);
        return new JsonResult(result);
    }

    // AJAX: GET /Reports?handler=ResponseTime
    public async Task<JsonResult> OnGetResponseTimeAsync(DateTime? startDate, DateTime? endDate)
    {
        var token = HttpContext.Session.GetString("Token");
        var result = await apiClient.GetResponseTimeAsync(startDate, endDate, token!);
        return new JsonResult(result);
    }

    //AJAX: GET /Reports?handler=Export
    public async Task<IActionResult> OnGetExportAsync([FromQuery] ManagerReportFilterDto filter)
    {
        var token = HttpContext.Session.GetString("Token");
        var fileBytes = await apiClient.ExportReportAsync(filter, token!);
        return File(fileBytes!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"TicketReport_{DateTime.UtcNow:yyyyMMdd}.xlsx");
    }

}
