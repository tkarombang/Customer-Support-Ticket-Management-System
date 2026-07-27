using TicketManagement.Application.Interfaces;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Reports;
using TicketManagement.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace TicketManagement.Application.Services;

public class ReportService(
    IReportRepository reportRepository,
    ITicketRepository ticketRepository)
    : IReportService
{

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync()
    {
        var tickets = (await ticketRepository.GetAllAsync()).ToList();

        var workload = tickets
            .Where(t => t.AssignedTo.HasValue && t.AssignedAgent != null)
            .GroupBy(t => new { t.AssignedTo, t.AssignedAgent!.Name })
            .Select(g => new AgentWorkloadDto
            {
                UserId = g.Key.AssignedTo!.Value,
                AgentName = g.Key.Name,
                AssignedTicketCount = g.Count()
            })
            .ToList();

        return new DashboardSummaryDto
        {
            TotalTickets = tickets.Count,
            OpenCount = tickets.Count(t => t.Status == TicketStatus.Open),
            InProgressCount = tickets.Count(t => t.Status == TicketStatus.InProgress),
            ResolvedCount = tickets.Count(t => t.Status == TicketStatus.Resolved),
            ClosedCount = tickets.Count(t => t.Status == TicketStatus.Closed),
            WorkloadPerAgent = workload
        };
    }

    public async Task<PagedResult<ManagerReportItemDto>> GetManagerReportAsync(ManagerReportFilterDto filter)
    {
        // REQ-3.2: query dibentuk sebagai IQueryable agar filter di-apply
        // sebelum eksekusi ke database (deferred execution + .AsNoTracking()
        // sudah diterapkan di level repository, lihat Phase 4).
        var query = reportRepository.GetFilterableQuery();

        if (filter.StartDate.HasValue)
            query = query.Where(t => t.CreatedDate >= filter.StartDate.Value);

        if (filter.EndDate.HasValue)
            query = query.Where(t => t.CreatedDate <= filter.EndDate.Value.Date.AddDays(1));

        if (!string.IsNullOrWhiteSpace(filter.Status)
            && Enum.TryParse<TicketStatus>(filter.Status.Replace(" ", ""), out var status))
        {
            query = query.Where(t => t.Status == status);
        }

        if (filter.AssignedToUserId.HasValue)
            query = query.Where(t => t.AssignedTo == filter.AssignedToUserId.Value);

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.Trim();
            query = query.Where(t =>
                t.TicketNumber.Contains(term) ||
                t.CustomerName.Contains(term) ||
                t.Title.Contains(term));
        }

        var totalCount = await query
            .CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedDate)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(t => new ManagerReportItemDto
            {
                TicketId = t.Id,
                TicketNumber = t.TicketNumber,
                CustomerName = t.CustomerName,
                CustomerEmail = t.CustomerEmail,
                Title = t.Title,
                Status = t.Status.ToString(),
                AssignedToUserId = t.AssignedTo,
                AssignedToAgentName = t.AssignedAgent != null ? t.AssignedAgent.Name : null,
                CreatedDate = t.CreatedDate,
                UpdatedDate = t.UpdatedDate
            })
            .ToListAsync();

        return new PagedResult<ManagerReportItemDto>
        {
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            Items = items
        };
    }

    
}