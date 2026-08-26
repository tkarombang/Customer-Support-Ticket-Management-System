using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketManagement.Application.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Users;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.Users.Base)]
[Authorize(Roles = RoleConstants.Administrator)]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        var users = await userService.GetAllAsync();
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create(CreateUserDto dto)
    {
        var userId = GetCurrentUserId();
        var created = await userService.CreateAsync(dto, userId);
        return CreatedAtAction(nameof(GetAll), created);
    }

    [HttpPut(ApiRoutes.Users.ById)]
    public async Task<ActionResult<UserResponseDto>> Update(Guid id, UpdateUserDto dto)
    {
        var userId = GetCurrentUserId();
        var updated = await userService.UpdateAsync(id, dto, userId);
        return Ok(updated);
    }

    [HttpPut(ApiRoutes.Users.ToggleStatus)]
    public async Task<ActionResult<UserResponseDto>> ToggleStatus(Guid id)
    {
        var updated = await userService.ToggleStatusAsync(id);
        return Ok(updated);
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("Invalid user id.");

        return userId;
    }
}