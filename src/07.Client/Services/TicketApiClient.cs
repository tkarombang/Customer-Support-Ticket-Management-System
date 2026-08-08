using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using TicketManagement.Client.Interfaces;
using TicketManagement.Shared.Constants;
using TicketManagement.Shared.Dtos.Auth;
using TicketManagement.Shared.Dtos.Profile;
using TicketManagement.Shared.Dtos.Reports;
using TicketManagement.Shared.Dtos.TicketHistories;
using TicketManagement.Shared.Dtos.Tickets;
using TicketManagement.Shared.Dtos.Users;
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

    public async Task<TicketResponseDto?> GetTicketByIdAsync(Guid id, string token)
    {
        AttachToken(token);
        return await httpClient.GetFromJsonAsync<TicketResponseDto>($"{ApiRoutes.Tickets.Base}/{id}", JsonOptions);
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

    public async Task<IEnumerable<UserResponseDto>?> GetUsersAsync(string token)
    {
        AttachToken(token);
        return await httpClient.GetFromJsonAsync<IEnumerable<UserResponseDto>>(
            $"{ApiRoutes.Users.Base}", JsonOptions);
    }

    public async Task<UserResponseDto?> CreateUserAsync(CreateUserDto dto, string token)
    {
        AttachToken(token);
        var response = await httpClient.PostAsJsonAsync($"{ApiRoutes.Users.Base}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserResponseDto>(JsonOptions);
    }

    public async Task<UserResponseDto?> UpdateUserAsync(Guid id, UpdateUserDto dto, string token)
    {
        AttachToken(token);
        var response = await httpClient.PutAsJsonAsync($"{ApiRoutes.Users.Base}/{id}", dto);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserResponseDto>(JsonOptions);
    }

    public async Task<UserResponseDto?> ToggleUserStatusAsync(Guid id, string token)
    {
        AttachToken(token);
        var response = await httpClient.PutAsync($"{ApiRoutes.Users.Base}/{id}/status", null);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<UserResponseDto>(JsonOptions);
    }

    public async Task<TicketAttachmentResponseDto?> UploadAttachmentAsync(
    Guid ticketId, Stream fileStream, string fileName, string contentType, string token)
    {
        AttachToken(token);

        using var content = new MultipartFormDataContent();
        using var streamContent = new StreamContent(fileStream);
        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
        content.Add(streamContent, "file", fileName);

        var response = await httpClient.PostAsync($"{ApiRoutes.Tickets.Base}/{ticketId}/attachments", content);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TicketAttachmentResponseDto>(JsonOptions);
    }

    public async Task<PagedResult<TicketHistoryItemDto>> GetTicketHistoriesAsync(TicketHistoryFilterDto filter, string token)
    {
        AttachToken(token);

        var query = HttpUtility.ParseQueryString(string.Empty);
        if (filter.StartDate.HasValue)
            query["StartDate"] = filter.StartDate.Value.ToString("O");
        if (filter.EndDate.HasValue)
            query["EndDate"] = filter.EndDate.Value.ToString("O");
        if (!string.IsNullOrWhiteSpace(filter.Action))
            query["Action"] = filter.Action;
        if (filter.UserId.HasValue)
            query["UserId"] = filter.UserId.Value.ToString();
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            query["SearchTerm"] = filter.SearchTerm;
        query["PageNumber"] = filter.PageNumber.ToString();
        query["PageSize"] = filter.PageSize.ToString();

        return await httpClient.GetFromJsonAsync<PagedResult<TicketHistoryItemDto>>(
            $"{ApiRoutes.TicketHistories.Base}?{query}", JsonOptions)
            ?? new PagedResult<TicketHistoryItemDto>();
    }


    public async Task<SlaComplianceDto?> GetSlaComplianceAsync(DateTime? startDate, DateTime? endDate, string token)
    {
        AttachToken(token);

        var query = HttpUtility.ParseQueryString(string.Empty);
        if (startDate.HasValue) query["startDate"] = startDate.Value.ToString("O");
        if (endDate.HasValue) query["endDate"] = endDate.Value.ToString("O");

        return await httpClient.GetFromJsonAsync<SlaComplianceDto>(
            $"{ApiRoutes.Reports.Base}/{ApiRoutes.Reports.SlaCompliance}?{query}", JsonOptions);
    }

    public async Task<ResponseTimeDto?> GetResponseTimeAsync(DateTime? startDate, DateTime? endDate, string token)
    {
        AttachToken(token);
        var query = HttpUtility.ParseQueryString(string.Empty);
        if (startDate.HasValue) query["startDate"] = startDate.Value.ToString("O");
        if (endDate.HasValue) query["endDate"] = endDate.Value.ToString("O");

        return await httpClient.GetFromJsonAsync<ResponseTimeDto>(
            $"{ApiRoutes.Reports.Base}/{ApiRoutes.Reports.ResponseTime}?{query}", JsonOptions);
    }


    public async Task<byte[]?> ExportReportAsync(ManagerReportFilterDto filter, string token)
    {
        AttachToken(token);
        var response = await httpClient.GetAsync($"{ApiRoutes.Reports.Base}/{ApiRoutes.Reports.Export}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

    public async Task<UserResponseDtoForProfile?> GetProfileAsync(string token)
    {
        AttachToken(token);

        return await httpClient.GetFromJsonAsync<UserResponseDtoForProfile?>(ApiRoutes.Profile.Base);
    }

    public async Task ChangePasswordAsync(ChangePasswordDto dto, string token)
    {
        AttachToken(token);
        var response = await httpClient.PutAsJsonAsync($"{ApiRoutes.Profile.Base}/{ApiRoutes.Profile.Password}", dto);

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<ActivityLogDto>> GetActivityLogAsync(string token)
    {
        AttachToken(token);
        var logs = await httpClient.GetFromJsonAsync<List<ActivityLogDto>>($"{ApiRoutes.Profile.Base}/{ApiRoutes.Profile.ActivityLog}");

        return logs ?? [];
    }


    public async Task UpdateProfileAsync(UpdateProfileDto dto, string token)
    {
        AttachToken(token);

        var response = await httpClient.PutAsJsonAsync<UpdateProfileDto>(ApiRoutes.Profile.Base, dto);
        response.EnsureSuccessStatusCode();
    }


    private void AttachToken(string token) =>
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
}