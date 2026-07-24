using System.Net.Sockets;
using TicketManagement.Base.Common;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Domain.Entities;

public class User : BaseEntity
{
    public required string Name { get; set; }

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public UserRole Role { get; set; }

    // Navigation property: tiket yang menjadi tanggung jawab user ini (jika Agent)
    public ICollection<Ticket> AssignedTickets { get; set; } = [];
}