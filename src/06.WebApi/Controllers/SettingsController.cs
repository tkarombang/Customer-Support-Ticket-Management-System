using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketManagement.Application.Interfaces;
using TicketManagement.Application.Services;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Settings;
using TicketManagement.Shared.Dtos.SystemLogs;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.Settings.Base)]
[Authorize(Roles = RoleConstants.Administrator)]
public class SettingsController(
    ISettingsService settingsService,
    IBackupService backupService,
    ISystemLogQueryService systemLogQueryService) : ControllerBase
{
    [HttpGet(ApiRoutes.Settings.General)]
    public async Task<IActionResult> GetGeneral()
    {
        var result = await settingsService.GetGeneralAsync();
        return Ok(result);
    }

    [HttpPut(ApiRoutes.Settings.General)]
    public async Task<IActionResult> UpdateGeneral(GeneralSettingDto dto)
    {
        await settingsService.UpdateGeneralAsync(dto, GetCurrentUserId());
        return NoContent();
    }

    [HttpGet(ApiRoutes.Settings.Sla)]
    public async Task<IActionResult> GetSla()
    {
        var result = await settingsService.GetSlaAsync();
        return Ok(result);
    }

    [HttpPut(ApiRoutes.Settings.Sla)]
    public async Task<IActionResult> UpdateSla(SlaSettingDto dto)
    {
        await settingsService.UpdateSlaAsync(dto, GetCurrentUserId());
        return NoContent();
    }

    [HttpGet(ApiRoutes.Settings.Integrations)]
    public async Task<IActionResult> GetIntegrations()
    {
        var result = await settingsService.GetIntegrationsAsync();
        return Ok(result);
    }

    [HttpPost(ApiRoutes.Settings.Integrations)]
    public async Task<IActionResult> CreateIntegration(CreateIntegrationDto dto)
    {
        var created = await settingsService.CreateIntegrationAsync(dto);
        return Ok(created);
    }

    [HttpPut(ApiRoutes.Settings.IntegrationById)]
    public async Task<IActionResult> UpdateIntegration(Guid id, UpdateIntegrationDto dto)
    {
        var updated = await settingsService.UpdateIntegrationAsync(id, dto);
        return Ok(updated);
    }

    [HttpPost(ApiRoutes.Settings.Backup)]
    public async Task<IActionResult> TriggerBackup()
    {
        var result = await backupService.TriggerManualBackupAsync(GetCurrentUserId());
        return Ok(result);
    }

    [HttpGet(ApiRoutes.Settings.Backup)]
    public async Task<IActionResult> GetBackupHistory()
    {
        var result = await backupService.GetHistoryAsync();
        return Ok(result);
    }

    [HttpPost(ApiRoutes.Settings.Restore)]
    [RequestSizeLimit(500_000_000)] // 500MB, backup file bisa besar
    public async Task<IActionResult> Restore(IFormFile file)
    {
        if (file.Length == 0) return BadRequest(new { message = "File backup tidak boleh kosong." });
        await using var stream = file.OpenReadStream();
        await backupService.RestoreAsync(stream, file.FileName, GetCurrentUserId());

        return Ok(new { message = "Restore berhasil. Aplikasi mungkin perlu di-restart." });
    }

    [HttpGet(ApiRoutes.Settings.SystemLogs)]
    public async Task<IActionResult> GetSystemLogs([FromQuery] SystemLogFilterDto filter)
    {
        var result = await systemLogQueryService.GetFilteredAsync(filter);
        return Ok(result);
    }

    private Guid GetCurrentUserId() =>
    Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

}