using TicketManagement.Shared.Dtos.Tickets;

namespace TicketManagement.Application.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<TicketResponseDto>> GetAllAsync();
    Task<TicketResponseDto> GetByIdAsync(int id);
    Task<TicketResponseDto> CreateAsync(CreateTicketDto dto);
    Task<TicketResponseDto> UpdateAsync(int id, UpdateTicketDto dto, int changedByUserId);
    Task<TicketResponseDto> AssignAsync(int id, AssignTicketDto dto, int changedByUserId);
}