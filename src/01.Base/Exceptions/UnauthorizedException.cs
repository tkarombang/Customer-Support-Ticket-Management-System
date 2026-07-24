namespace TicketManagement.Base.Exceptions;

/// <summary>
/// Dilempar ketika user mencoba mengakses resource/aksi yang tidak diizinkan
/// untuk role-nya (contoh: Support Agent mencoba assign tiket).
/// Ditangkap oleh ExceptionHandlingMiddleware dan diterjemahkan ke HTTP 403.
/// </summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message = "Anda tidak memiliki akses untuk melakukan aksi ini.")
        : base(message)
    {
    }
}