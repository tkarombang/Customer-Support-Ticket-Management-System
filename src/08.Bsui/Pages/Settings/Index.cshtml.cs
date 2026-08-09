using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;

namespace TicketManagement.Bsui.Pages.Settings;
public class IndexModel(ITicketApiClient apiClient) : PageModel
{
    private string Token => HttpContext.Session.GetString("Token")!;
    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetString("Role") != "Administrator")
            return RedirectToPage("/Login");
        return Page();
    }

    public async Task<JsonResult> OnGetGeneralAsync()
    {
        return new(await apiClient.GetGeneralSettingAsync(Token));
    }
}
