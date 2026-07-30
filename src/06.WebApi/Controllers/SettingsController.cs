using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Application.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Settings;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.Settings.Base)]
[Authorize(Roles = RoleConstants.Administrator)]
public class SettingsController(ISettingsService settingsService) : ControllerBase
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

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}