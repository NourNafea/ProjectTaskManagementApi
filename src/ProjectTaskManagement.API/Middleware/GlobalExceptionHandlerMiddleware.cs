using System.Net;
using System.Text.Json;
using FluentValidation;
using ProjectTaskManagement.Application.Common;
using ProjectTaskManagement.Domain.Exceptions;

namespace ProjectTaskManagement.API.Middleware;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, response) = exception switch
        {
            ValidationException ve => (
                HttpStatusCode.BadRequest,
                ApiResponse.Fail("Validation failed.", ve.Errors.Select(e => e.ErrorMessage))),
            NotFoundException nfe => (
                HttpStatusCode.NotFound,
                ApiResponse.Fail(nfe.Message)),
            ForbiddenException => (
                HttpStatusCode.Forbidden,
                ApiResponse.Fail("Access denied.")),
            BadRequestException bre => (
                HttpStatusCode.BadRequest,
                ApiResponse.Fail(bre.Message)),
            _ => (
                HttpStatusCode.InternalServerError,
                ApiResponse.Fail("An unexpected error occurred."))
        };

        context.Response.StatusCode = (int)statusCode;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
