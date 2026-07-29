using TicketManagement.Base.Common;

namespace TicketManagement.Domain.Entities;

public class IntegrationConfig : BaseEntity
{
    public required string Name { get; set; }
    public string? WebhookUrl { get; set; }
    public string? ApiKeyEncrypted { get; set; }
    public bool IsActive { get; set; }
}