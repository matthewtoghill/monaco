using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Monaco.Template.Backend.Common.Api.Middleware;

public class GlobalErrorHandlerMiddleware
{
	private readonly RequestDelegate _next;
	private readonly ILogger _logger;

	public GlobalErrorHandlerMiddleware(RequestDelegate next, ILogger logger)
	{
		_next = next;
		_logger = logger;
	}

	public async Task Invoke(HttpContext context)
	{
		try
		{
			await _next(context);
		}
		catch (Exception ex)
		{
			await HandleExceptionAsync(context, ex);
			_logger.Error(ex, "An error occurred: {ErrorMessage}", ex.Message);
		}
	}

	private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
	{
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

		var errorResponse = new
		{
			Message = $"An error occurred: {exception.Message}"
		};

		await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
	}
}
