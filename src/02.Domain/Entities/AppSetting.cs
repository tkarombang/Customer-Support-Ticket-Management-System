namespace TicketManagement.Domain.Entities;

public class AppSetting
{
    public required string SettingKey { get; set; } // Primary Key
    public string? SettingValue { get; set; }
    public bool IsEncrypted { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
    public Guid? UpdatedBy { get; set; }

    public User? UpdatedByUser { get; set; }
}