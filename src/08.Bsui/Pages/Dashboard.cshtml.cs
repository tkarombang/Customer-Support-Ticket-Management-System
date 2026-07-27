using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;

namespace TicketManagement.Bsui.Pages;

public class DashboardModel(ITicketApiClient apiClient) : PageModel
{
    public IActionResult OnGet()
    {
        // Guard: hanya Manager yang boleh akses halaman ini
        if (HttpContext.Session.GetString("Role") != "Manager")
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
}