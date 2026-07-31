using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Application.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.TicketHistories;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.TicketHistories.Base)]
[Authorize(Roles = RoleConstants.Administrator)]
public class TicketHistoriesController(ITicketHistoryService historyService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFiltered([FromQuery] TicketHistoryFilterDto filter)
    {
        var result = await historyService.GetFilteredAsync(filter);
        return Ok(result);
    }
}