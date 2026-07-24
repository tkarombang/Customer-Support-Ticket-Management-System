namespace TicketManagement.Base.Exceptions;

/// <summary>
/// Dilempar ketika entity yang dicari (misal Ticket, User) tidak ditemukan di database.
/// Ditangkap oleh ExceptionHandlingMiddleware dan diterjemahkan ke HTTP 404.
/// </summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string entityName, object key)
        : base($"{entityName} dengan ID '{key}' tidak ditemukan.")
    {
    }
}