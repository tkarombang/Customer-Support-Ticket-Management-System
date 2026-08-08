using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;

namespace TicketManagement.Bsui.Pages.Profile;
public class IndexModel(ITicketApiClient apiClient) : PageModel
{
    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToPage("/Login");
        return Page();
    }

    // AJAX GET: /Profile?handler=Detail
    public async Task<JsonResult> OnGetDetailAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        var profile = await apiClient.GetProfileAsync(token!);
        return new JsonResult(profile);
    }
}

