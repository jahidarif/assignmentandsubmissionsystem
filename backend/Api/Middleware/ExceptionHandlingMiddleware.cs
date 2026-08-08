using System.Net;
using System.Text.Json;
using AssignmentSubmissionSystem.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using ValidationException = AssignmentSubmissionSystem.Application.Common.Exceptions.ValidationException;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for {Path}.", context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, new
            {
                title = "Validation failed",
                status = 400,
                errors = ex.Errors
            });
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning(ex, "Unauthorized on {Path}: {Message}", context.Request.Path, ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.Unauthorized, new
            {
                title = "Unauthorized",
                status = 401,
                message = ex.Message
            });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Not found on {Path}: {Message}", context.Request.Path, ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.NotFound, new
            {
                title = "Not found",
                status = 404,
                message = ex.Message
            });
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning(ex, "Conflict on {Path}: {Message}", context.Request.Path, ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.Conflict, new
            {
                title = "Conflict",
                status = 409,
                message = ex.Message
            });
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Database constraint violation on {Path}.", context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.Conflict, new
            {
                title = "Conflict",
                status = 409,
                message = "This record can't be modified because other data depends on it."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Path}.", context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, new
            {
                title = "An unexpected error occurred.",
                status = 500
            });
        }
    }

    private static Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, object body)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(body));
    }
}