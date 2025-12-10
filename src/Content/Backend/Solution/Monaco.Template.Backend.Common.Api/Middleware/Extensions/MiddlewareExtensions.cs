using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Monaco.Template.Backend.Common.Api.Middleware.Extensions;

public static class MiddlewareExtensions
{
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Adds the Serilog context enricher middleware to the service collection for dependency injection.
		/// </summary>
		/// <remarks>This method registers <see cref="SerilogContextEnricherMiddleware"/> with a scoped lifetime. Call
		/// this method during application startup to enable Serilog context enrichment for each request.</remarks>
		/// <returns>The same <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
		public IServiceCollection AddSerilogContextEnricher() =>
			services.AddScoped<SerilogContextEnricherMiddleware>();

		/// <summary>
		/// Adds the JwtClaimsMapperMiddleware to the service collection for dependency injection.
		/// </summary>
		/// <returns>The current IServiceCollection instance with the JwtClaimsMapperMiddleware registered.</returns>
		public IServiceCollection AddJwtClaimsMapper() =>
			services.AddScoped<JwtClaimsMapperMiddleware>();
	}
	
	/// <param name="app"></param>
	extension(IApplicationBuilder app)
	{
		/// <summary>
		/// Uses the Serilog Context Enricher middleware to inject the current user into the Serilog Context.
		/// </summary>
		/// <returns></returns>
		public IApplicationBuilder UseSerilogContextEnricher() =>
			app.UseMiddleware<SerilogContextEnricherMiddleware>();

		/// <summary>
		/// Uses a middleware for mapping all claims from a JWT token to the Context.User but without running any kind of authentication/authorization middleware
		/// </summary>
		/// <returns></returns>
		public IApplicationBuilder UseJwtClaimsMapper() =>
			app.UseMiddleware<JwtClaimsMapperMiddleware>();
	}
}