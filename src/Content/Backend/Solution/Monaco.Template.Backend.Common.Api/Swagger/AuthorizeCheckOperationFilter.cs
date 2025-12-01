using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Net;

namespace Monaco.Template.Backend.Common.Api.Swagger;

public class AuthorizeCheckOperationFilter : IOperationFilter
{
	private readonly string _audience;

	public AuthorizeCheckOperationFilter(string audience)
	{
		_audience = audience;
	}

	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		if (context.ApiDescription
				   .ActionDescriptor
				   .EndpointMetadata
				   .Any(m => m is IAllowAnonymous))
			return;

		operation.Responses ??= [];

		var unauthorizedKey = ((int)HttpStatusCode.Unauthorized).ToString();
		if (!operation.Responses.ContainsKey(unauthorizedKey))
			operation.Responses.Add(unauthorizedKey, new OpenApiResponse { Description = HttpStatusCode.Unauthorized.ToString() });

		var forbiddenKey = ((int)HttpStatusCode.Forbidden).ToString();
		if (!operation.Responses.ContainsKey(forbiddenKey))
			operation.Responses.Add(forbiddenKey, new OpenApiResponse { Description = HttpStatusCode.Forbidden.ToString() });

		var oAuthScheme = new OpenApiSecuritySchemeReference("oauth2", context.Document);

		operation.Security = [new OpenApiSecurityRequirement { [oAuthScheme] = [_audience] }];
	}
}