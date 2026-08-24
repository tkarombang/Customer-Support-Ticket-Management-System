using TicketManagement.Shared.Dtos.Auth;

namespace TicketManagement.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto dto, string? ipAddress = null);
}