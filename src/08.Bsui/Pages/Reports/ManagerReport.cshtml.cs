using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Reports;

namespace TicketManagement.Bsui.Pages.Reports;

public class ManagerReportModel(ITicketApiClient apiClient) : PageModel
{
    public IActionResult OnGet()
    {
        // Guard sederhana: hanya Manager yang boleh akses halaman ini
        if (HttpContext.Session.GetString("Role") != "Manager")
            return RedirectToPage("/Login");

        return Page();
    }

    // Dipanggil oleh manager-report.js via AJAX: GET /Reports/ManagerReport?handler=Filter&...
    public async Task<JsonResult> OnGetFilterAsync([FromQuery] ManagerReportFilterDto filter)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        var result = await apiClient.GetManagerReportAsync(filter, token);
        return new JsonResult(result);
    }
}