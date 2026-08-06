using TicketManagement.Application.Interfaces;
using TicketManagement.Domain.Enums;
using TicketManagement.Domain.Interfaces;
using TicketManagement.Shared.Dtos.Reports;
using TicketManagement.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TicketManagement.Shared.Dtos.Settings;
using TicketManagement.Domain.Entities;

namespace TicketManagement.Application.Services;

public class ReportService(
    IReportRepository reportRepository,
    ITicketRepository ticketRepository,
    IAppSettingRepository appSettingRepository)
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



    public async Task<SlaComplianceDto> GetSlaCompliannceAsync(DateTime? startDate, DateTime? endDate)
    {
        var slaSetting = await GetSlaSettingAsync();

        var query = reportRepository.GetFilterableQuery()
            .Where(t => t.Status == TicketStatus.Resolved || t.Status == TicketStatus.Closed);

        if (startDate.HasValue) query = query.Where(t => t.CreatedDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(t => t.CreatedDate <= endDate.Value);

        var resolvedTickets = await query.ToListAsync();
        var totalResolved = resolvedTickets.Count;

        var withinSla = resolvedTickets.Count(t =>
        {
            var targetHours = t.Priority switch
            {
                TicketPriority.High => slaSetting.HighPriorityHours,
                TicketPriority.Medium => slaSetting.MediumPriorityHours,
                _ => slaSetting.LowPriorityHours
            };

            var actualHours = t.UpdatedDate.HasValue
                ? (t.UpdatedDate.Value - t.CreatedDate).TotalHours
                : double.MaxValue;

            return actualHours <= targetHours;
        });

        var compliancePercentage = totalResolved == 0 ? 0 : Math.Round((double)withinSla / totalResolved * 100, 1);

        return new SlaComplianceDto
        {
            CompliancePercentage = compliancePercentage,
            TotalResolved = totalResolved,
            WithinSla = withinSla,
            BreachedSla = totalResolved - withinSla,
            Trend = []
        };
    }


    public async Task<ResponseTimeDto> GetResponseTimeAsync(DateTime? startDate, DateTime? endDate)
    {
        var query = reportRepository.GetFilterableQuery()
            .Where(t => t.UpdatedDate.HasValue);

        if (startDate.HasValue) query = query.Where(t => t.CreatedDate >= startDate.Value);
        if (endDate.HasValue) query = query.Where(t => t.CreatedDate <= endDate.Value);

        var tickets = await query.ToListAsync();

        var avgHours = tickets.Count == 0
            ? 0
            : tickets.Average(t => (t.UpdatedDate!.Value - t.CreatedDate).TotalHours);

        return new ResponseTimeDto
        {
            AverageResponseHours = Math.Round(avgHours, 1),
            AverageResponseHoursPreviousPeriod = 0 // opsional: bandingkan periode sebelumnya, di-skip untuk kesederhanaan
        };
    }



    private async Task<SlaSettingDto> GetSlaSettingAsync()
    {
        var setting = await appSettingRepository.GetByKeyAsync("Sla.Config");
        return setting?.SettingValue is null
            ? new SlaSettingDto()
            : System.Text.Json.JsonSerializer.Deserialize<SlaSettingDto>(setting.SettingValue)!;
    }

    
}