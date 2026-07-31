using Microsoft.EntityFrameworkCore;
using TicketManagement.Application.Interfaces;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.SystemLogs;
using TicketManagement.Shared.Models;

namespace TicketManagement.Application.Services
{
    public class SystemLogQueryService(ISystemLogRepository systemLogRepository) : ISystemLogQueryService
    {
        public async Task<PagedResult<SystemLogItemDto>> GetFilteredAsync(SystemLogFilterDto filter)
        {
            var query = systemLogRepository.GetFilterableQuery();

            if (filter.StartDate.HasValue) query = query.Where(l => l.Timestamp >= filter.StartDate.Value);
            if (filter.EndDate.HasValue) query = query.Where(l => l.Timestamp <= filter.EndDate.Value);
            if (filter.UserId.HasValue) query = query.Where(l => l.UserId == filter.UserId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Action)
                && Enum.TryParse<SystemLogAction>(filter.Action, out var action))
            {
                query = query.Where(l => l.Action == action);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                query = query.Where(l => l.Description.Contains(filter.SearchTerm));

            var totalCount = await query.CountAsync();
            var items = await query
                .OrderByDescending(l => l.Timestamp)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(l => new SystemLogItemDto
                {
                    LogId = l.Id,
                    UserName = l.User != null ? l.User.Name : null,
                    Action = l.Action.ToString(),
                    Description = l.Description,
                    IpAddress = l.IpAddress,
                    Timestamp = l.Timestamp
                })
                .ToListAsync();

            return new PagedResult<SystemLogItemDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = items
            };
        }
    }
}
