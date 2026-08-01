using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Users;

namespace TicketManagement.Bsui.Pages.Users;

public class IndexModel(ITicketApiClient apiClient) : PageModel
{
    public IActionResult OnGet()
    {
        // Guard: hanya Administrator yang boleh akses halaman ini
        if (HttpContext.Session.GetString("Role") != "Administrator")
            return RedirectToPage("/Login");

        return Page();
    }

    // AJAX: GET /Users?handler=List
    public async Task<JsonResult> OnGetListAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        var users = await apiClient.GetUsersAsync(token);
        return new JsonResult(users);
    }

    // AJAX: POST /Users?handler=Create
    public async Task<JsonResult> OnPostCreateAsync([FromBody] CreateUserDto dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        try
        {
            var created = await apiClient.CreateUserAsync(dto, token);
            return new JsonResult(created);
        }
        catch (HttpRequestException ex)
        {
            return new JsonResult(new { error = ex.Message }) { StatusCode = 400 };
        }
    }

    // AJAX: PUT /Users?handler=Update&id=...
    public async Task<JsonResult> OnPutUpdateAsync(Guid id, [FromBody] UpdateUserDto dto)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        try
        {
            var updated = await apiClient.UpdateUserAsync(id, dto, token);
            return new JsonResult(updated);
        }
        catch (HttpRequestException ex)
        {
            return new JsonResult(new { error = ex.Message }) { StatusCode = 400 };
        }
    }

    // AJAX: PUT /Users?handler=ToggleStatus&id=...
    public async Task<JsonResult> OnPutToggleStatusAsync(Guid id)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrEmpty(token)) return new JsonResult(new { error = "Unauthorized" }) { StatusCode = 401 };

        var updated = await apiClient.ToggleUserStatusAsync(id, token);
        return new JsonResult(updated);
    }
}