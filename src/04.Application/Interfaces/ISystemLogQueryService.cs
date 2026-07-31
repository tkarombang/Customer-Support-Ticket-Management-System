using TicketManagement.Shared.Dtos.SystemLogs;
using TicketManagement.Shared.Models;

namespace TicketManagement.Application.Interfaces
{
    public interface ISystemLogQueryService
    {
        Task<PagedResult<SystemLogItemDto>> GetFilteredAsync(SystemLogFilterDto filter);
    }
}
