using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

namespace Monaco.Template.Backend.Common.Api.OpenApi;

public static class OpenApiExtensions
{
	extension(IServiceCollection services)
	{
#if (auth)
		public IServiceCollection AddOpenApiDocs(string authEndpoint,
												 string tokenEndpoint,
												 string audience,
												 List<string> scopesList) =>
#else
		public IServiceCollection AddOpenApiDocs() =>
#endif
			services.AddApiVersioning(options =>
									  {
										  options.ReportApiVersions = true;
										  options.DefaultApiVersion = new ApiVersion(1, 0);
										  options.AssumeDefaultVersionWhenUnspecified = true;
										  options.ApiVersionReader = new UrlSegmentApiVersionReader();
									  })
					.AddApiExplorer(options =>
									{
										options.GroupNameFormat = "'v'VVV";
										options.SubstituteApiVersionInUrl = true;
									})
					.Services
#if (auth)
					.AddOpenApi(opts => opts.AddDocumentTransformer(new OAuth2DocumentTransformer(authEndpoint,
																								  tokenEndpoint,
																								  audience,
																								  scopesList))
											.AddOperationTransformer(new OAuth2OperationTransformer(audience)));
#else
					.AddOpenApi();
#endif
	}

	extension(WebApplication app)
	{
#if (auth)
		public IApplicationBuilder UseOpenApiDocs(string title,
												  string authEndpoint,
												  string tokenEndpoint,
												  string clientId,
												  string audience,
												  List<string> scopesList)
#else
		public IApplicationBuilder UseOpenApiDocs(string title)
#endif
		{
			app.MapOpenApi();
#if (auth)
			app.MapScalarApiReference(opts => opts.WithTitle(title)
												  .AddAuthorizationCodeFlow("OAuth2",
																			flow => flow.WithAuthorizationUrl(authEndpoint)
																						.WithTokenUrl(tokenEndpoint)
																						.WithPkce(Pkce.Sha256)
																						.WithClientId(clientId)
																						.WithSelectedScopes([..scopesList, audience])));
#else
			app.MapScalarApiReference(opts => opts.WithTitle(title));
#endif
			return app;
		}
	}
}