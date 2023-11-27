﻿using Microsoft.AspNetCore.Builder;

namespace Monaco.Template.Backend.Common.Api.Middleware.Extensions;

public static class MiddlewareExtensions
{
	/// <summary>
	/// Uses the Serilog Context Enricher middleware to inject the current user into the Serilog Context.
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static IApplicationBuilder UseSerilogContextEnricher(this IApplicationBuilder app) =>
		app.UseMiddleware<SerilogContextEnricherMiddleware>();

	/// <summary>
	/// Uses a middleware for mapping all claims from a JWT token to the Context.User but without running any kind of authentication/authorization middleware
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static IApplicationBuilder UseJwtClaimsMapper(this IApplicationBuilder app) =>
		app.UseMiddleware<JwtClaimsMapperMiddleware>();

	/// <summary>
	/// Uses a middleware for handling any unhandled errors. The errors are logged and a message is returned.
	/// </summary>
	/// <param name="app"></param>
	/// <returns></returns>
	public static IApplicationBuilder UseGlobalErrorHandler(this IApplicationBuilder app) =>
		app.UseMiddleware<GlobalErrorHandlerMiddleware>();
}