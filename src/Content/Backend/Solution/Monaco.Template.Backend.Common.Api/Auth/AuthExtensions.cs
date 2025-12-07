using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;

namespace Monaco.Template.Backend.Common.Api.Auth;

public static class AuthExtensions
{
	public const string ScopeClaimType = "scope";

	extension(IServiceCollection services)
	{
		public IServiceCollection AddAuthorizationWithPolicies(List<string> scopes) =>
			services.AddAuthorization(cfg =>
									  {   // DefaultPolicy will require at least authenticated user by default
										  cfg.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
															  .RequireAuthenticatedUser().Build();
										  // Register all listed scopes as policies requiring the existence of such scope in User claims
										  scopes.ForEach(s => cfg.AddPolicy(s, p => p.RequireScope(s)));
									  });

		public AuthenticationBuilder AddJwtBearerAuthentication(string authority,
																string audience,
																bool requireHttpsMetadata) =>
			services.AddTransient<IClaimsTransformation, ScopeClaimsTransformation>() // Add transformer to map scopes correctly in ClaimsPrincipal/Identity
					.AddAuthentication()
					.AddJwtBearer(options => // Configure validation settings for JWT bearer
								  {
									  options.Authority = authority;
									  options.Audience = audience;
									  options.RequireHttpsMetadata = requireHttpsMetadata;
									  options.TokenValidationParameters.NameClaimType = "name";
									  options.TokenValidationParameters.RoleClaimType = "roles";

									  options.TokenHandlers.Clear();
									  options.TokenHandlers.Add(new JwtSecurityTokenHandler { MapInboundClaims = false });

									  options.TokenValidationParameters.ValidTypes = ["JWT"];
								  });
	}

	extension(AuthorizationPolicyBuilder builder)
	{
		/// <summary>
		/// Requires claims of type "scope" with matching values
		/// </summary>
		/// <param name="allowedValues"></param>
		/// <returns></returns>
		public AuthorizationPolicyBuilder RequireScope(params string[] allowedValues) =>
			builder.RequireClaim(ScopeClaimType, (IEnumerable<string>)allowedValues);
	}
}