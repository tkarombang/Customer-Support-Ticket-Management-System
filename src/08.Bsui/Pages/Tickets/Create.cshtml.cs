using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Tickets;

namespace TicketManagement.Bsui.Pages.Tickets;

public class CreateModel(ITicketApiClient apiClient) : PageModel
{
    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToPage("/Login");

        return Page();
    }

    // AJAX: GET /Tickets/Create?handler=Agents (dropdown Assign To & CC)
    public async Task<JsonResult> OnGetAgentsAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        var users = await apiClient.GetUsersAsync(token);
        var agents = users?.Where(u => u.Role == "Agent" && u.Status == "Active");
        return new JsonResult(agents);
    }

    // AJAX: POST /Tickets/Create?handler=Submit
    public async Task<JsonResult> OnPostSubmitAsync([FromBody] CreateTicketDto dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        try
        {
            var created = await apiClient.CreateTicketAsync(dto, token);
            return new JsonResult(created);
        }
        catch (HttpRequestException ex)
        {
            return new JsonResult(new { error = ex.Message }) { StatusCode = 400 };
        }
    }

    // AJAX: POST /Tickets/Create?handler=UploadAttachment&ticketId=...
    public async Task<JsonResult> OnPostUploadAttachmentAsync(Guid ticketId, IFormFile file)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        await using var stream = file.OpenReadStream();
        var result = await apiClient.UploadAttachmentAsync(ticketId, stream, file.FileName, file.ContentType, token);
        return new JsonResult(result);
    }
}