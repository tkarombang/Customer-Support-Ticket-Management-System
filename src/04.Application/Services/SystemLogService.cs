using System.Security.Cryptography;
using TicketManagement.Domain.Entities;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TicketManagement.Application.Services;

public interface ISystemLogService
{
    Task LogAsync(Guid? userId, SystemLogAction action, string description, string? ipAddress = null);
}

public class SystemLogService(ISystemLogRepository systemLogRepository) : ISystemLogService
{
    public async Task LogAsync(Guid? userId, SystemLogAction action, string description, string? ipAddress = null)
    {
        await systemLogRepository.AddAsync(new SystemLog
        {
            UserId = userId,
            Action = action,
            Description = description,
            IpAddress = ipAddress
        });
    }
}

//Ini service kecil yang sebaiknya di-inject ke service 
//lain(AuthService untuk log Login, TicketService untuk log 
//CreateTicket, dst) daripada tiap service manggil ISystemLogRepository 
//langsung — supaya konsisten formatnya.Saya sarankan nanti 
//kita refactor ProfileService di atas untuk pakai ISystemLogService ini juga, 
//bukan ISystemLogRepository langsung.