using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Profile;

namespace TicketManagement.Bsui.Pages.Profile;
public class IndexModel(ITicketApiClient apiClient) : PageModel
{
    public IActionResult OnGet()
    {
        if (string.IsNullOrEmpty(HttpContext.Session.GetString("Token")))
            return RedirectToPage("/Login");
        return Page();
    }

    // AJAX GET: /Profile?handler=Detail
    public async Task<JsonResult> OnGetDetailAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        var profile = await apiClient.GetProfileAsync(token!);
        return new JsonResult(profile);
    }


    // AJAX PUT: /Profile?handler=ChangePassword'
    public async Task<JsonResult> OnPutChangePasswordAsync([FromBody] ChangePasswordDto dto)
    {
        var token = HttpContext.Session.GetString("Token");
        try
        {
            await apiClient.ChangePasswordAsync(dto, token!);
            return new JsonResult(new { success = true });
        }
        catch (Exception)
        {
            return new JsonResult(new { error = "Password Lama Tidak Sesuai atau Terjadi Kesalahan." }) { StatusCode = 400 };
        }
    }

    public async Task<JsonResult> OnGetActivityLogAsync()
    {
        var token = HttpContext.Session.GetString("Token");
        var logs = await apiClient.GetActivityLogAsync(token!);
        return new JsonResult(logs);
    }


}

