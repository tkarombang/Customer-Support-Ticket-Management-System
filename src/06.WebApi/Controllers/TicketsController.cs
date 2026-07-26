using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TicketManagement.Application.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Tickets;

namespace TicketManagement.WebApi.Controllers;

[ApiController]
[Route(ApiRoutes.Tickets.Base)]
[Authorize] // REQ-1.1: semua endpoint tiket butuh login
public class TicketsController(ITicketService ticketService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetAll()
    {
        var tickets = await ticketService.GetAllAsync();
        return Ok(tickets);
    }

    [HttpGet(ApiRoutes.Tickets.ById)]
    public async Task<ActionResult<TicketResponseDto>> GetById(Guid id)
    {
        var ticket = await ticketService.GetByIdAsync(id);
        return Ok(ticket);
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponseDto>> Create(CreateTicketDto dto)
    {
        var created = await ticketService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.TicketId }, created);
    }

    [HttpPut(ApiRoutes.Tickets.ById)]
    public async Task<ActionResult<TicketResponseDto>> Update(Guid id, UpdateTicketDto dto)
    {
        var userId = GetCurrentUserId();
        var updated = await ticketService.UpdateAsync(id, dto, userId);
        return Ok(updated);
    }

    [HttpPut(ApiRoutes.Tickets.Assign)]
    [Authorize(Roles = RoleConstants.Manager)] // REQ-2.6: hanya Manager
    public async Task<ActionResult<TicketResponseDto>> Assign(Guid id, AssignTicketDto dto)
    {
        var userId = GetCurrentUserId();
        var assigned = await ticketService.AssignAsync(id, dto, userId);
        return Ok(assigned);
    }

    private Guid GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
            throw new UnauthorizedAccessException("Invalid user id.");

        return userId;
    }
}