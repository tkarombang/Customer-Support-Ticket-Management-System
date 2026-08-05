using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.TicketHistories;

namespace TicketManagement.Bsui.Pages.TicketHistories;

public class IndexModel(ITicketApiClient apiClient) : PageModel
{
    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("Role") != "Administrator")
            return RedirectToPage("/Login");

        return Page();
    }

    // AJAX: GET /TicketHistories?handler=Filter
    public async Task<JsonResult> OnGetFilterAsync([FromQuery] TicketHistoryFilterDto filter)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        var result = await apiClient.GetTicketHistoriesAsync(filter, token);
        return new JsonResult(result);
    }

    // AJAX: GET /TicketHistories?handler=Users (dropdown filter "Semua User")
    public async Task<JsonResult> OnGetUsersAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        var users = await apiClient.GetUsersAsync(token!);

        return new JsonResult(users);
    }
}
