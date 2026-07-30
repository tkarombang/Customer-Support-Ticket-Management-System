using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Shared.Dtos.TicketHistories;
using TicketManagement.Shared.Models;

namespace TicketManagement.Application.Interfaces
{
    public interface ITicketHistoryService
    {
        Task<PagedResult<TicketHistoryItemDto>> GetFilteredAsync(TicketHistoryFilterDto filter);
    }
}
