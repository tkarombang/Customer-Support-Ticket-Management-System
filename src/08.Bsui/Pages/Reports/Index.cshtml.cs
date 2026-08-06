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

    public async Task<JsonResult> OnGetSlaAsync(DateTime? startDate, DateTime? endDate)
    {
        var token = HttpContext.Session.GetString("Token");
        var result = await apiClient.GetSlaComplianceAsync(startDate, endDate, token!);
        return new JsonResult(result);
    }

}
