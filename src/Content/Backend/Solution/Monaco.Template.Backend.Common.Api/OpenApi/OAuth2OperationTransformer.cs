using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Monaco.Template.Backend.Common.Api.OpenApi;

/// <summary>
/// Applies per-operation OAuth2 security requirements based on endpoint authorization metadata.
/// </summary>
public class OAuth2OperationTransformer : IOpenApiOperationTransformer
{
	private readonly string _audience;

	public OAuth2OperationTransformer(string audience)
	{
		_audience = audience;
	}

	public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
	{
		var metadata = context.Description.ActionDescriptor.EndpointMetadata;

		if (metadata.Any(m => m is IAllowAnonymous))
		{
			operation.Security = [];
			return Task.CompletedTask;
		}

		var requiredScopes = metadata.OfType<IAuthorizeData>()
									 .Where(a => !string.IsNullOrWhiteSpace(a.Policy))
									 .Select(a => a.Policy!)
									 .Distinct()
									 .ToList();

		operation.Security =
		[
			new()
			{
				[new(OAuth2DocumentTransformer.SchemeName,
					 context.Document)] = [..requiredScopes, _audience]
			}
		];

		return Task.CompletedTask;
	}
}