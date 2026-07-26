using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Auth;

namespace TicketManagement.Bsui.Pages;

public class LoginModel(ITicketApiClient apiClient) : PageModel
{
    [BindProperty]
    public LoginRequestDto Input { get; set; } = new() { Email = "", Password = "" };

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        try
        {
            var result = await apiClient.LoginAsync(Input);
            if (result is null)
            {
                ErrorMessage = "Login gagal.";
                return Page();
            }

            HttpContext.Session.SetString("Token", result.Token);
            HttpContext.Session.SetString("Role", result.Role);
            HttpContext.Session.SetString("Name", result.Name);

            return result.Role == "Manager"
                ? RedirectToPage("/Reports/ManagerReport")
                : RedirectToPage("/Tickets/Index");
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Email atau password salah.";
            return Page();
        }
    }
}