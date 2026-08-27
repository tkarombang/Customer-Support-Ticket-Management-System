using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Settings;
using TicketManagement.Shared.Dtos.SystemLogs;

namespace TicketManagement.Bsui.Pages.Settings;
public class IndexModel(ITicketApiClient apiClient) : PageModel
{
    private string Token => HttpContext.Session.GetString("Token")!;
    public IActionResult OnGet(string? tab)
    {
        if (HttpContext.Session.GetString("Role") != "Administrator")
            return RedirectToPage("/Login");

        ViewData["InitialTab"] = tab ?? "general";
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


    // AJAX PUT (INTEGRATION):BELUM ADA UI DAN AJAX NYA



    // AJAX GET: /Settings?handler=BackupHistory
    public async Task<JsonResult> OnGetBackupHistoryAsync()
    {
        return new(await apiClient.GetBackupHistoryAsync(Token));
    }

    // AJAX POST: /Settings?handler=Backup
    public async Task<JsonResult> OnPostBackupAsync()
    {
        var result = await apiClient.TriggerBackupAsync(Token);
        return new JsonResult(result);
    }

    // AJAX POST: /Settings?handler=Restore
    public async Task<JsonResult> OnPostRestoreAsync(IFormFile file)
    {
        await using var stream = file.OpenReadStream();
        await apiClient.RestoreBackupAsync(stream, file.FileName, Token);
        return new JsonResult(new { success = true, message = "Restore berhasil. Aplikasi mungkin perlu di-restart." });
    }


    // AJAX POST: /Settings?handler=SystemLogs
    public async Task<JsonResult> OnGetSystemLogsAsync([FromQuery] SystemLogFilterDto filter)
    {
        var result = await apiClient.GetSystemLogsAsync(filter, Token);
        return new JsonResult(result);
    }


}
