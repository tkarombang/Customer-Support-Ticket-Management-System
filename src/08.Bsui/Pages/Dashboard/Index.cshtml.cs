using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Reports;

namespace TicketManagement.Bsui.Pages.Dashboard
{
    public class IndexModel(ITicketApiClient apiClient) : PageModel
    {
        public IActionResult OnGet()
        {
            // Guard: hanya Manager yang boleh akses halaman ini
            if (HttpContext.Session.GetString("Role") != "Administrator")
                return RedirectToPage("/Login");

            return Page();
        }

        // AJAX: GET /Dashboard?handler=Summary
        public async Task<JsonResult> OnGetSummaryAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

            var summary = await apiClient.GetDashboardSummaryAsync(token);
            return new JsonResult(summary);
        }

        // AJAX: GET /Dashboard?handler=TrendData
        public async Task<JsonResult> OnGetTrendDataAsync()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

            var filter = new ManagerReportFilterDto { PageNumber = 1, PageSize = 200 };
            var result = await apiClient.GetManagerReportAsync(filter, token);
            return new JsonResult(result?.Items);
        }
    }
}
