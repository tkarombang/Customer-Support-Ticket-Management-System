namespace TicketManagement.Shared.Dtos.Settings
{
    public class SlaSettingDto
    {
        public int HighPriorityHours { get; set; } = 4;
        public int MediumPriorityHours { get; set; } = 24;
        public int LowPriorityHours { get; set; } = 72;
    }
}
