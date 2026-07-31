namespace TicketManagement.Shared.Dtos.SystemLogs
{
    public class SystemLogItemDto
    {
        public Guid LogId { get; set; }
        public string? UserName { get; set; }
        public required string Action { get; set; }
        public required string Description { get; set; }
        public string? IpAddress { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
