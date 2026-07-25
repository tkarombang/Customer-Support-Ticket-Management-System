using TicketManagement.Application.Interfaces;
using TicketManagement.Base.Exceptions;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Tickets;

namespace TicketManagement.Application.Services;

public class TicketService(
    ITicketRepository ticketRepository,
    IUserRepository userRepository)
    : ITicketService
{
    public async Task<IEnumerable<TicketResponseDto>> GetAllAsync()
    {
        var tickets = await ticketRepository.GetAllAsync();
        return tickets.Select(MapToDto);
    }

    public async Task<TicketResponseDto> GetByIdAsync(int id)
    {
        var ticket = await ticketRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Ticket", id);
        return MapToDto(ticket);
    }

    public async Task<TicketResponseDto> CreateAsync(CreateTicketDto dto)
    {
        // REQ-2.2: Auto-generate TicketNumber format TKT-00001
        var lastSequence = await ticketRepository.GetLastTicketSequenceAsync();
        var ticketNumber = $"TKT-{(lastSequence + 1):D5}";

        var ticket = new Ticket
        {
            TicketNumber = ticketNumber,
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            Title = dto.Title,
            Description = dto.Description,
            Status = TicketStatus.Open // REQ-2.3
        };

        var created = await ticketRepository.AddAsync(ticket);
        return MapToDto(created);
    }

    public async Task<TicketResponseDto> UpdateAsync(int id, UpdateTicketDto dto, int changedByUserId)
    {
        var ticket = await ticketRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Ticket", id);

        // REQ-2.5: Closed ticket tidak boleh dimodifikasi
        if (!ticket.IsModifiable())
            throw new ValidationException("Status", "Tiket berstatus Closed tidak dapat diubah.");

        if (!Enum.TryParse<TicketStatus>(dto.Status.Replace(" ", ""), out var newStatus))
            throw new ValidationException("Status", $"Status '{dto.Status}' tidak valid.");

        var previousStatus = ticket.Status;
        ticket.Description = dto.Description;
        ticket.Status = newStatus;
        ticket.UpdatedDate = DateTime.UtcNow;

        // REQ-2.8: catat perubahan status ke TicketHistory
        if (previousStatus != newStatus)
        {
            ticket.Histories.Add(new TicketHistory
            {
                TicketId = ticket.Id,
                Action = "StatusChanged",
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                ChangedBy = changedByUserId
            });
        }

        await ticketRepository.UpdateAsync(ticket);
        return MapToDto(ticket);
    }

    public async Task<TicketResponseDto> AssignAsync(int id, AssignTicketDto dto, int changedByUserId)
    {
        var ticket = await ticketRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Ticket", id);

        if (!ticket.IsModifiable())
            throw new ValidationException("Status", "Tiket berstatus Closed tidak dapat di-assign ulang.");

        // REQ-2.6: assignee harus user terdaftar dengan role SupportAgent
        var isValidAgent = await userRepository.ExistsWithRoleAsync(dto.AssignedToUserId, UserRole.SupportAgent);
        if (!isValidAgent)
            throw new ValidationException("AssignedToUserId", "User tidak ditemukan atau bukan Support Agent.");

        ticket.AssignedTo = dto.AssignedToUserId;
        ticket.UpdatedDate = DateTime.UtcNow;

        // REQ-2.8: catat assignment ke TicketHistory
        ticket.Histories.Add(new TicketHistory
        {
            TicketId = ticket.Id,
            Action = "Assigned",
            ChangedBy = changedByUserId
        });

        await ticketRepository.UpdateAsync(ticket);
        return MapToDto(ticket);
    }

    private static TicketResponseDto MapToDto(Ticket ticket) => new()
    {
        TicketId = ticket.Id,
        TicketNumber = ticket.TicketNumber,
        CustomerName = ticket.CustomerName,
        CustomerEmail = ticket.CustomerEmail,
        Title = ticket.Title,
        Description = ticket.Description,
        Status = ticket.Status.ToString(),
        AssignedToUserId = ticket.AssignedTo,
        AssignedToAgentName = ticket.AssignedAgent?.Name,
        CreatedDate = ticket.CreatedDate,
        UpdatedDate = ticket.UpdatedDate
    };
}