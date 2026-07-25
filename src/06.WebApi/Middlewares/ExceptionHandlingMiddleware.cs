using System.Net;
using System.Text.Json;
using TicketManagement.Base.Exceptions;

namespace TicketManagement.WebApi.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message, null),
            ValidationException validationEx =>
                (HttpStatusCode.BadRequest, exception.Message, (object?)validationEx.Errors),
            UnauthorizedException => (HttpStatusCode.Forbidden, exception.Message, null),
            _ => (HttpStatusCode.InternalServerError, "Terjadi kesalahan pada server.", null)
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new { message, errors };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}