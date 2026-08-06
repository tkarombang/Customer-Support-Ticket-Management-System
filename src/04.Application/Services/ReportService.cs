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

        bool IsWithinSla(Domain.Entities.Ticket t)
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
        }

        var totalResolved = resolvedTickets.Count;
        var withinSla = resolvedTickets.Count(IsWithinSla);
        var compliancePercentage = totalResolved == 0 ? 0 : Math.Round((double)withinSla / totalResolved * 100, 1);

        var trend = resolvedTickets
            .Where(t => t.UpdatedDate.HasValue)
            .GroupBy(t => t.UpdatedDate!.Value.Date)
            .OrderBy(g => g.Key)
            .Select(g => new SlaComplianceTrendPointDto
            {
                Date = g.Key,
                CompliancePercentage = Math.Round((double)g.Count(IsWithinSla) / g.Count() * 100, 1)
            }).ToList();

        return new SlaComplianceDto
        {
            CompliancePercentage = compliancePercentage,
            TotalResolved = totalResolved,
            WithinSla = withinSla,
            BreachedSla = totalResolved - withinSla,
            Trend = trend
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


    public async Task<byte[]> ExportToExcelAsync(ManagerReportFilterDto filter)
    {
        filter.PageSize = int.MaxValue; // export semua hasil filter, bukan 1 halaman saja
        var data = await GetManagerReportAsync(filter);

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var sheet = workbook.Worksheets.Add("Tickets Report");

        // Header
        string[] headers = ["Ticket Number", "Customer", "Title", "Status", "Priority", "Assignee", "Created Date"];
        for (int i = 0; i < headers.Length; i++)
            sheet.Cell(1, i + 1).Value = headers[i];

        // Rows
        int row = 2;
        foreach (var item in data.Items)
        {
            sheet.Cell(row, 1).Value = item.TicketNumber;
            sheet.Cell(row, 2).Value = item.CustomerName;
            sheet.Cell(row, 3).Value = item.Title;
            sheet.Cell(row, 4).Value = item.Status;
            sheet.Cell(row, 5).Value = item.AssignedToAgentName ?? "-";
            sheet.Cell(row, 6).Value = item.CreatedDate.ToString("yyyy-MM-dd");
            row++;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }



    private async Task<SlaSettingDto> GetSlaSettingAsync()
    {
        var setting = await appSettingRepository.GetByKeyAsync("Sla.Config");
        return setting?.SettingValue is null
            ? new SlaSettingDto()
            : System.Text.Json.JsonSerializer.Deserialize<SlaSettingDto>(setting.SettingValue)!;
    }

    
}