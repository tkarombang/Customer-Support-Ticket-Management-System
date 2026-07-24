namespace TicketManagement.Base.Exceptions;

/// <summary>
/// Dilempar ketika business rule / validasi input gagal
/// (contoh: format email salah, tiket Closed tidak boleh diubah).
/// Ditangkap oleh ExceptionHandlingMiddleware dan diterjemahkan ke HTTP 400.
/// </summary>
public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(string field, string errorMessage)
        : base("Satu atau lebih validasi gagal.")
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { errorMessage } }
        };
    }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("Satu atau lebih validasi gagal.")
    {
        Errors = errors;
    }
}