using System.Net;
using System.Text.Json;

namespace PresupuestoApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado");

            ctx.Response.ContentType = "application/json";
            (int status, string mensaje) = ex switch
            {
                UnauthorizedAccessException => ((int)HttpStatusCode.Unauthorized, ex.Message),
                KeyNotFoundException => ((int)HttpStatusCode.NotFound, ex.Message),
                InvalidOperationException => ((int)HttpStatusCode.BadRequest, ex.Message),
                ArgumentException => ((int)HttpStatusCode.BadRequest, ex.Message),
                _ => ((int)HttpStatusCode.InternalServerError, "Error interno del servidor")
            };

            ctx.Response.StatusCode = status;
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                error = mensaje,
                statusCode = status
            }));
        }
    }
}
