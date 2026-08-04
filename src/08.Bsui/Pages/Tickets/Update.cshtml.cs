using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Tickets;

namespace TicketManagement.Bsui.Pages.Tickets;

public class UpdateModel(ITicketApiClient apiClient) : PageModel
{
    public Guid TicketId { get; set; }

    public IActionResult OnGet(Guid id)
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToPage("/Login");

        TicketId = id;
        return Page();
    }

    // AJAX: GET /Tickets/Update?handler=Detail&id=...
    public async Task<JsonResult> OnGetDetailAsync(Guid id)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        var ticket = await apiClient.GetTicketByIdAsync(id, token); // perlu ditambahkan di ITicketApiClient
        return new JsonResult(ticket);
    }

    public async Task<JsonResult> OnGetAgentsAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        var users = await apiClient.GetUsersAsync(token!);
        return new JsonResult(users?.Where(u => u.Role == "Agent" && u.Status == "Active"));
    }

    public async Task<JsonResult> OnPutSubmitAsync(Guid id, [FromBody] UpdateTicketDto dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        try
        {
            var updated = await apiClient.UpdateTicketAsync(id, dto, token);
            return new JsonResult(updated);
        }
        catch (HttpRequestException ex)
        {
            return new JsonResult(new { error = ex.Message }) { StatusCode = 400 };
        }
    }

    public async Task<JsonResult> OnPutAssignAsync(Guid id, [FromBody] AssignTicketDto dto)
    {
        var token = HttpContext.Session.GetString("Token");
        var role = HttpContext.Session.GetString("Role");
        if (role != "Administrator") return new JsonResult(new { error = "Forbidden" }) { StatusCode = 403 };

        var assigned = await apiClient.AssignTicketAsync(id, dto, token!);
        return new JsonResult(assigned);
    }
}