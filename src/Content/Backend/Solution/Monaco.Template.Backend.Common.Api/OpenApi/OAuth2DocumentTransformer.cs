using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Monaco.Template.Backend.Common.Api.OpenApi;

/// <summary>
/// Adds OAuth2 security scheme to the OpenAPI document components.
/// </summary>
public sealed class OAuth2DocumentTransformer : IOpenApiDocumentTransformer
{
	public const string SchemeName = "OAuth2";
	
	private readonly string _authorizationUrl;
	private readonly string _tokenUrl;
	private readonly string _audience;
	private readonly IReadOnlyList<string> _scopes;

	public OAuth2DocumentTransformer(string authorizationUrl,
									 string tokenUrl,
									 string audience,
									 IReadOnlyList<string> scopes)
	{
		_authorizationUrl = authorizationUrl;
		_tokenUrl = tokenUrl;
		_audience = audience;
		_scopes = scopes;
	}

	public Task TransformAsync(OpenApiDocument document,
							   OpenApiDocumentTransformerContext context,
							   CancellationToken cancellationToken)
	{
		document.Components ??= new();
		document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>([
			new(SchemeName,
				new OpenApiSecurityScheme
				{
					Type = SecuritySchemeType.OAuth2,
					Flows = new()
							{
								AuthorizationCode = new()
													{
														AuthorizationUrl = new(_authorizationUrl),
														TokenUrl = new(_tokenUrl),
														Scopes = new Dictionary<string, string>(_scopes.ToDictionary(k => k, _ => "[No description]"))
																 {
																	 { _audience, "API Audience" }
																 }
													}
							}
				})
		]);

		document.Security = [new() { [new(SchemeName, document)] = [] }];
		
		// Set the host document for all elements
		// including the security scheme references
		document.SetReferenceHostDocument();

		return Task.CompletedTask;
	}
}