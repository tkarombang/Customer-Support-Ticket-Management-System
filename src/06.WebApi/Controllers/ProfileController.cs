using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Application.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Profile;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.Profile.Base)]
[Authorize] // semua role authenticated boleh akses profile sendiri
public class ProfileController(IProfileService profileService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var userId = GetCurrentUserId();
        var profile = await profileService.GetAsync(userId);
        return Ok(profile);
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateProfileDto dto)
    {
        var userId = GetCurrentUserId();
        await profileService.UpdateAsync(userId, dto);
        return NoContent();
    }

    [HttpPut(ApiRoutes.Profile.Password)]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = GetCurrentUserId();
        await profileService.ChangePasswordAsync(userId, dto);
        return NoContent();
    }

    [HttpGet(ApiRoutes.Profile.ActivityLog)]
    public async Task<IActionResult> GetActivityLog()
    {
        var userId = GetCurrentUserId();
        var logs = await profileService.GetActivityLogAsync(userId);
        return Ok(logs);
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}