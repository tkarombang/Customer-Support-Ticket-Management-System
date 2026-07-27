using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Dtos.Auth;
using TicketManagement.Shared.Dtos.Reports;
using TicketManagement.Shared.Dtos.Tickets;
using TicketManagement.Shared.Models;

namespace TicketManagement.Client.Services;

public class TicketApiClient(HttpClient httpClient) : ITicketApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var response = await httpClient.PostAsJsonAsync("api/auth/login", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<LoginResponseDto>(JsonOptions);
    }

    public async Task<IEnumerable<TicketResponseDto>?> GetTicketsAsync(string token)
    {
        AttachToken(token);
        return await httpClient.GetFromJsonAsync<IEnumerable<TicketResponseDto>>("api/tickets");
    }

    public async Task<TicketResponseDto?> CreateTicketAsync(CreateTicketDto dto, string token)
    {
        AttachToken(token);
        var response = await httpClient.PostAsJsonAsync("api/tickets", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TicketResponseDto>(JsonOptions);
    }

    public async Task<TicketResponseDto?> UpdateTicketAsync(Guid id, UpdateTicketDto dto, string token)
    {
        AttachToken(token);
        var response = await httpClient.PutAsJsonAsync($"api/tickets/{id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TicketResponseDto>(JsonOptions);
    }

    public async Task<TicketResponseDto?> AssignTicketAsync(Guid id, AssignTicketDto dto, string token)
    {
        AttachToken(token);
        var response = await httpClient.PutAsJsonAsync($"api/tickets/{id}/assign", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TicketResponseDto>(JsonOptions);
    }

    public async Task<PagedResult<ManagerReportItemDto>?> GetManagerReportAsync(ManagerReportFilterDto filter, string token)
    {
        AttachToken(token);

        var query = HttpUtility.ParseQueryString(string.Empty);
        if (filter.StartDate.HasValue) query["StartDate"] = filter.StartDate.Value.ToString("O");
        if (filter.EndDate.HasValue) query["EndDate"] = filter.EndDate.Value.ToString("O");
        if (!string.IsNullOrWhiteSpace(filter.Status)) query["Status"] = filter.Status;
        if (filter.AssignedToUserId.HasValue) query["AssignedToUserId"] = filter.AssignedToUserId.Value.ToString();
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm)) query["SearchTerm"] = filter.SearchTerm;
        query["PageNumber"] = filter.PageNumber.ToString();
        query["PageSize"] = filter.PageSize.ToString();

        return await httpClient.GetFromJsonAsync<PagedResult<ManagerReportItemDto>>($"api/reports/manager-report?{query}");
    }

    public async Task<DashboardSummaryDto?> GetDashboardSummaryAsync(string token)
    {
        AttachToken(token);
        return await httpClient.GetFromJsonAsync<DashboardSummaryDto>("api/dashboard/summary");
    }

    private void AttachToken(string token) =>
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}