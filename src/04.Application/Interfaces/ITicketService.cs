using Microsoft.AspNetCore.Http;
using TicketManagement.Shared.Dtos.Tickets;

namespace TicketManagement.Application.Interfaces;

public interface ITicketService
{
    Task<IEnumerable<TicketResponseDto>> GetAllAsync();
    Task<TicketResponseDto> GetByIdAsync(Guid id);
    Task<TicketResponseDto> CreateAsync(CreateTicketDto dto, Guid CreatedBy);
    Task<TicketResponseDto> UpdateAsync(Guid id, UpdateTicketDto dto, Guid changedByUserId);
    Task<TicketResponseDto> AssignAsync(Guid id, AssignTicketDto dto, Guid changedByUserId);
    // ITicketService.cs — tambahan
    Task<TicketAttachmentResponseDto> UploadAttachmentAsync(Guid ticketId, IFormFile file, Guid uploadedBy);
}