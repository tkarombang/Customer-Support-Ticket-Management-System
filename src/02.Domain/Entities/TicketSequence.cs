namespace TicketManagement.Domain.Entities;

/// <summary>
/// Tabel counter terpisah untuk generate TicketNumber (TKT-00001) secara sequential,
/// karena Ticket.Id sekarang Guid dan tidak punya urutan alami.
/// Didesain sebagai single-row table (LastSequence terus di-increment).
/// </summary>
public class TicketSequence
{
    public int Id { get; set; }
    public int Year { get; set; }
    public int LastSequence { get; set; } = 0;
}