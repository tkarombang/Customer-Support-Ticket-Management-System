using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Settings;

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

    // AJAX GET: /Settings?handler=General
    public async Task<JsonResult> OnGetGeneralAsync()
    {
        return new(await apiClient.GetGeneralSettingAsync(Token));
    }
    // AJAX PUT: /Settings?handler=General
    public async Task<JsonResult> OnPutGeneralAsync([FromBody] GeneralSettingDto dto)
    {
        await apiClient.UpdateGeneralSettingAsync(dto, Token);
        return new JsonResult(new { success = true });
    }



    // AJAX GET: /Settings?handler=Sla
    public async Task<JsonResult> OnGetSlaAsync()
    {
        return new(await apiClient.GetSlaSettingAsync(Token));
    }
    // AJAX PUT: /Settings?handler=Sla
    public async Task<JsonResult> OnPutSlaAsync([FromBody] SlaSettingDto dto)
    {
        await apiClient.UpdateSlaSettingAsync(dto, Token);
        return new JsonResult(new { success = true });
    }


    // AJAX GET: /Settings?handler=Integrations
    public async Task<JsonResult> OnGetIntegrationsAsync()
    {
        var result = await apiClient.GetIntegrationsAsync(Token);
        return new JsonResult(result);
    }

    // AJAX POST: /Settings?handler=Integration
    public async Task<JsonResult> OnPostIntegrationAsync([FromBody] CreateIntegrationDto dto)
    {
        var result = await apiClient.CreateIntegrationAsync(dto, Token);
        return new JsonResult(result);
    }


}
