using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TicketManagement.Application.Interfaces;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.TicketHistories;
using TicketManagement.Shared.Models;

namespace TicketManagement.Application.Services
{
    public class TicketHistoryService(ITicketHistoryRepository historyRepository) : ITicketHistoryService
    {
        public async Task<PagedResult<TicketHistoryItemDto>> GetFilteredAsync(TicketHistoryFilterDto filter)
        {
            // REQ-8.1: filter tanggal, aksi, user, dan search (nomor tiket/judul)
            var query = historyRepository.GetFilterableQuery();

            if (filter.StartDate.HasValue)
                query = query.Where(h => h.Timestamp >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(h => h.Timestamp <= filter.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.Action)
                && Enum.TryParse<HistoryAction>(filter.Action, out var action))
            {
                query = query.Where(h => h.Action == action);
            }

            if (filter.UserId.HasValue)
                query = query.Where(h => h.ChangedBy == filter.UserId.Value);

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim();
                query = query.Where(h =>
                    h.Ticket != null && (
                        h.Ticket.TicketNumber.Contains(term) ||
                        h.Ticket.Title.Contains(term)
                    ));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(h => h.Timestamp)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(h => new TicketHistoryItemDto
                {
                    HistoryId = h.Id,
                    TicketNumber = h.Ticket != null ? h.Ticket.TicketNumber : "-",
                    Action = h.Action.ToString(),
                    PreviousStatus = h.PreviousStatus != null ? h.PreviousStatus.ToString() : null,
                    NewStatus = h.NewStatus != null ? h.NewStatus.ToString() : null,
                    ChangedByName = h.ChangedByUser != null ? h.ChangedByUser.Name : "Unknown",
                    Timestamp = h.Timestamp
                }).ToListAsync();


            return new PagedResult<TicketHistoryItemDto>
            {
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize,
                Items = items
            };

        }

    }
}
