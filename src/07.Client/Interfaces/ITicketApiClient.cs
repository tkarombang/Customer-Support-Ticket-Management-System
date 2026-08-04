using TicketManagement.Shared.Dtos.Auth;
using TicketManagement.Shared.Dtos.Reports;
using TicketManagement.Shared.Dtos.Tickets;
using TicketManagement.Shared.Dtos.Users;
using TicketManagement.Shared.Models;

namespace TicketManagement.Client.Interfaces;

public interface ITicketApiClient
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<IEnumerable<TicketResponseDto>?> GetTicketsAsync(string token);
    Task<TicketResponseDto?> CreateTicketAsync(CreateTicketDto dto, string token);
    Task<TicketResponseDto?> UpdateTicketAsync(Guid id, UpdateTicketDto dto, string token);
    Task<TicketResponseDto?> AssignTicketAsync(Guid id, AssignTicketDto dto, string token);
    Task<PagedResult<ManagerReportItemDto>?> GetManagerReportAsync(ManagerReportFilterDto filter, string token);
    Task<DashboardSummaryDto?> GetDashboardSummaryAsync(string token);
    Task<IEnumerable<UserResponseDto>?> GetUsersAsync(string token);
    Task<UserResponseDto?> CreateUserAsync(CreateUserDto dto, string token);
    Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto, string token);
    Task<UserResponseDto?> ToggleUserStatusAsync(Guid id, string token);
    Task<TicketAttachmentResponseDto?> UploadAttachmentAsync(Guid ticketId, Stream fileStream, string fileName, string contentType, string token);
    Task<TicketResponseDto?> GetTicketByIdAsync(Guid id, string token);
}