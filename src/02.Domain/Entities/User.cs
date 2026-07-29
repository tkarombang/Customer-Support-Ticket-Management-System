using TicketManagement.Base.Common;
using TicketManagement.Domain.Enums;

namespace TicketManagement.Domain.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public string? PhoneNumber { get; set; }
    public string? JobTitle { get; set; }
    public string? Address { get; set; }
    public string? AvatarUrl { get; set; }

    public ICollection<Ticket> AssignedTickets { get; set; } = [];
}