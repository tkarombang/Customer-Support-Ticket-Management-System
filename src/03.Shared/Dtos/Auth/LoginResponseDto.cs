namespace TicketManagement.Shared.Dtos.Auth;

public class LoginResponseDto
{
    public required string Token { get; set; }
    public required string Name { get; set; }
    public required string Role { get; set; }
    public DateTime ExpiresAt { get; set; }
}