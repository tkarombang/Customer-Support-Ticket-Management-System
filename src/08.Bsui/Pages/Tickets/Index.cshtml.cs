using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Tickets;

namespace TicketManagement.Bsui.Pages.Tickets;


[IgnoreAntiforgeryToken]
public class IndexModel(ITicketApiClient apiClient) : PageModel
{
    public string? Role { get; set; }

    public IActionResult OnGet()
    {
        // Guard: harus login dulu
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToPage("/Login");

        Role = HttpContext.Session.GetString("Role");
        return Page();
    }

    // AJAX: GET /Tickets?handler=List
    public async Task<JsonResult> OnGetListAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        var tickets = await apiClient.GetTicketsAsync(token);
        return new JsonResult(tickets);
    }

    // AJAX: POST /Tickets?handler=Create

    public async Task<JsonResult> OnPostCreateAsync([FromBody] CreateTicketDto dto)
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

    // AJAX: PUT /Tickets?handler=Update&id=1
    public async Task<JsonResult> OnPutUpdateAsync(Guid id, [FromBody] UpdateTicketDto dto)
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

    // AJAX: PUT /Tickets?handler=Assign&id=1 (khusus Manager)
    public async Task<JsonResult> OnPutAssignAsync(Guid id, [FromBody] AssignTicketDto dto)
    {
        var token = HttpContext.Session.GetString("Token");
        var role = HttpContext.Session.GetString("Role");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };
        if (role != "Manager") return new JsonResult(new { error = "Forbidden" }) { StatusCode = 403 };

        try
        {
            var assigned = await apiClient.AssignTicketAsync(id, dto, token);
            return new JsonResult(assigned);
        }
        catch (HttpRequestException ex)
        {
            return new JsonResult(new { error = ex.Message }) { StatusCode = 400 };
        }
    }
}