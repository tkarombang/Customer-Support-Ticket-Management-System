namespace TicketManagement.Domain.Entities;

public class TicketCc
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }

    public Ticket? Ticket { get; set; }
    public User? User { get; set; }
}