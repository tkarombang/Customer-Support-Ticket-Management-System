using Microsoft.AspNetCore.Http;
using TicketManagement.Application.Interfaces;
using TicketManagement.Base.Exceptions;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Tickets;

namespace TicketManagement.Application.Services;

public class TicketService(
    ITicketRepository ticketRepository,
    IUserRepository userRepository,
    IFileStorageService fileStorageService,
    ITicketAttachmentRepository attachmentRepository,
    ITicketSequenceRepository ticketSequenceRepository,
    ISystemLogService systemLogService)
    : ITicketService
{
    public async Task<IEnumerable<TicketResponseDto>> GetAllAsync()
    {
        var tickets = await ticketRepository.GetAllAsync();
        return tickets.Select(MapToDto);
    }

    public async Task<TicketResponseDto> GetByIdAsync(Guid id)
    {
        var ticket = await ticketRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Ticket", id);
        return MapToDto(ticket);
    }

    public async Task<TicketResponseDto> CreateAsync(CreateTicketDto dto, Guid CreatedBy)
    {
        // REQ-2.2: Auto-generate TicketNumber format TKT-00001
        var currentYear = DateTime.UtcNow.Year;
        var nextSequence = await ticketSequenceRepository.GetNextSequenceAsync(currentYear);
        var ticketNumber = $"TKT-{currentYear}-{nextSequence:D4}";

        if (!Enum.TryParse<TicketType>(dto.Type, out var type))
            throw new ValidationException("Type", $"Tipe '{dto.Type}' tidak valid.");
        if (!Enum.TryParse<TicketImpact>(dto.Impact, out var impact))
            throw new ValidationException("Impact", $"Impact '{dto.Impact}' tidak valid.");
        if (!Enum.TryParse<TicketCategory>(dto.Category, out var category))
            throw new ValidationException("Category", $"Kategori '{dto.Category}' tidak valid.");
        if (!Enum.TryParse<TicketPriority>(dto.Priority, out var priority))
            throw new ValidationException("Priority", $"Prioritas '{dto.Priority}' tidak valid.");

        var ticket = new Ticket
        {
            TicketNumber = ticketNumber,
            Type = type,
            Impact = impact,
            Category = category,
            ApplicationSystem = dto.ApplicationSystem,
            Priority = priority,
            DueDate = dto.DueDate,
            CustomerName = dto.CustomerName,
            CustomerEmail = dto.CustomerEmail,
            Title = dto.Title,
            Description = dto.Description,
            Status = TicketStatus.Open,
            AssignedTo = dto.AssignedToUserId
        };

        var created = await ticketRepository.AddAsync(ticket);


        if (dto.CcUserIds is { Count: > 0 })
        {
            foreach (var userId in dto.CcUserIds)
                created.CcUsers.Add(new TicketCc { TicketId = created.Id, UserId = userId });
            await ticketRepository.UpdateAsync(created);
        }

        await systemLogService.LogAsync(CreatedBy, SystemLogAction.CreateTicket, "Berhasil Membuat Tiket");

        return MapToDto(created);
    }

    public async Task<TicketResponseDto> UpdateAsync(Guid id, UpdateTicketDto dto, Guid changedByUserId)
    {
        var ticket = await ticketRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Ticket", id);

        // REQ-2.5: Closed ticket tidak boleh dimodifikasi
        if (!ticket.IsModifiable())
            throw new ValidationException("Status", "Tiket berstatus Closed tidak dapat diubah.");

        if (!Enum.TryParse<TicketStatus>(dto.Status.Replace(" ", ""), out var newStatus))
            throw new ValidationException("Status", $"Status '{dto.Status}' tidak valid.");
        if (!Enum.TryParse<TicketType>(dto.Type, out var type))
            throw new ValidationException("Type", $"Tipe '{dto.Type}' tidak valid.");
        if (!Enum.TryParse<TicketImpact>(dto.Impact, out var impact))
            throw new ValidationException("Impact", $"Impact '{dto.Impact}' tidak valid.");
        if (!Enum.TryParse<TicketCategory>(dto.Category, out var category))
            throw new ValidationException("Category", $"Kategori '{dto.Category}' tidak valid.");
        if (!Enum.TryParse<TicketPriority>(dto.Priority, out var newPriority))
            throw new ValidationException("Priority", $"Prioritas '{dto.Priority}' tidak valid.");

        var previousStatus = ticket.Status;
        var previousPriority = ticket.Priority;

        ticket.Type = type;
        ticket.Impact = impact;
        ticket.Category = category;
        ticket.ApplicationSystem = dto.ApplicationSystem;
        ticket.Priority = newPriority;
        ticket.DueDate = dto.DueDate;
        ticket.Title = dto.Title;
        ticket.Description = dto.Description;
        ticket.Status = newStatus;
        ticket.UpdatedDate = DateTime.UtcNow;

        // REQ-2.8: catat perubahan status ke TicketHistory
        if (previousStatus != newStatus)
        {
            ticket.Histories.Add(new TicketHistory
            {
                TicketId = ticket.Id,
                Action = HistoryAction.StatusChanged,
                PreviousStatus = previousStatus,
                NewStatus = newStatus,
                ChangedBy = changedByUserId
            });
        }

        // Histori perubahan prioritas (REQ-8.2)
        if (previousPriority != newPriority)
        {
            ticket.Histories.Add(new TicketHistory
            {
                TicketId = ticket.Id,
                Action = HistoryAction.PriorityChanged,
                ChangedBy = changedByUserId
            });
        }

        // Catatan opsional jadi comment (REQ-2.13)
        if (!string.IsNullOrWhiteSpace(dto.StatusNote))
        {
            ticket.Comments.Add(new TicketComment
            {
                TicketId = ticket.Id,
                Content = dto.StatusNote,
                CreatedBy = changedByUserId
            });

            ticket.Histories.Add(new TicketHistory
            {
                TicketId = ticket.Id,
                Action = HistoryAction.CommentAdded,
                ChangedBy = changedByUserId
            });
        }

        await ticketRepository.UpdateAsync(ticket);
        return MapToDto(ticket);
    }

    public async Task<TicketResponseDto> AssignAsync(Guid id, AssignTicketDto dto, Guid changedByUserId)
    {
        var ticket = await ticketRepository.GetByIdAsync(id)
            ?? throw new NotFoundException("Ticket", id);

        if (!ticket.IsModifiable())
            throw new ValidationException("Status", "Tiket berstatus Closed tidak dapat di-assign ulang.");

        // REQ-2.6: assignee harus user terdaftar dengan role SupportAgent
        var isValidAgent = await userRepository.ExistsWithRoleAsync(dto.AssignedToUserId, UserRole.Agent);
        if (!isValidAgent)
            throw new ValidationException("AssignedToUserId", "User tidak ditemukan atau bukan Support Agent.");

        ticket.AssignedTo = dto.AssignedToUserId;
        ticket.UpdatedDate = DateTime.UtcNow;

        // REQ-2.8: catat assignment ke TicketHistory
        ticket.Histories.Add(new TicketHistory
        {
            TicketId = ticket.Id,
            Action = HistoryAction.StatusChanged,
            ChangedBy = changedByUserId
        });

        await ticketRepository.UpdateAsync(ticket);
        return MapToDto(ticket);
    }

    public async Task<TicketAttachmentResponseDto> UploadAttachmentAsync(Guid ticketId, IFormFile file, Guid uploadedBy)
    {
        var ticket = await ticketRepository.GetByIdAsync(ticketId)
            ?? throw new NotFoundException("Ticket", ticketId);

        var (filePath, fileSize) = await fileStorageService.SaveAsync(file, "attachments");

        var attachment = new TicketAttachment
        {
            TicketId = ticketId,
            FileName = file.FileName,
            FilePath = filePath,
            FileSizeBytes = fileSize,
            ContentType = file.ContentType,
            UploadedBy = uploadedBy
        };

        await attachmentRepository.AddAsync(attachment);

        return new TicketAttachmentResponseDto
        {
            AttachmentId = attachment.Id,
            FileName = attachment.FileName,
            FilePath = attachment.FilePath,
            FileSizeBytes = attachment.FileSizeBytes,
            UploadedByName = "-", // diisi nanti kalau perlu join user
            UploadedDate = attachment.CreatedDate
        };
    }

    private static TicketResponseDto MapToDto(Ticket ticket) => new()
    {
        Type = ticket.Type.ToString(),
        Impact = ticket.Impact.ToString(),
        Category = ticket.Category.ToString(),
        ApplicationSystem = ticket.ApplicationSystem,
        Priority = ticket.Priority.ToString(),
        DueDate = ticket.DueDate,
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