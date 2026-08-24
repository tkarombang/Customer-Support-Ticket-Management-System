using Microsoft.AspNetCore.Mvc;
using TicketManagement.Application.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Auth;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.Auth.Base)]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost(ApiRoutes.Auth.Login)]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto dto)
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        var result = await authService.LoginAsync(dto, ipAddress);
        return Ok(result);
    }
}