using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Application.Interfaces
{
    public interface ISystemLogService
    {
        Task LogAsync(Guid? userId, SystemLogAction action, string description, string? ipAddress = null);
    }
}
