namespace TicketManagement.Base.Common;

/// <summary>
/// Base class untuk seluruh entity domain. Menyediakan properti umum
/// (Id, CreatedDate, UpdatedDate) agar tidak duplikasi di tiap entity.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }
}