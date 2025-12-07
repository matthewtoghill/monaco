using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Monaco.Template.Backend.Common.Application.Extensions;

public static class ClaimsPrincipalExtensions
{
	private const string ResourceAccessClaimName = "resource_access";
	private static readonly JsonSerializerOptions JsonSerializerOptions = new(JsonSerializerDefaults.Web);

	extension(ClaimsPrincipal principal)
	{
		/// <summary>
		/// Retrieves the User Id from the "sub" claim
		/// </summary>
		/// <returns></returns>
		public Guid? GetUserId() =>
			principal.HasClaim(c => c.Type == JwtRegisteredClaimNames.Sub)
				? Guid.Parse(principal.FindFirst(JwtRegisteredClaimNames.Sub)!.Value)
				: null;

		/// <summary>
		/// Determines if the user has the specified role on the specified client
		/// </summary>
		/// <param name="clientName"></param>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public bool IsInClientRole(string clientName, string roleName)
		{
			var resourceAccessClaim = principal.FindFirst(ResourceAccessClaimName);
			if (resourceAccessClaim is null)
				return false;

			var clients = JsonSerializer.Deserialize<Dictionary<string, JsonObject>>(resourceAccessClaim.Value,
																					 JsonSerializerOptions);
			return clients is not null &&
				   clients.ContainsKey(clientName) &&
				   (clients[clientName][principal.Identities.First().RoleClaimType]?.Deserialize<string[]>()?.Contains(roleName) ?? false);
		}

		/// <summary>
		/// Determines if the user has the specified role in the client specified by the Audience (aud) claim
		/// </summary>
		/// <param name="roleName"></param>
		/// <returns></returns>
		public bool IsInClientRole(string roleName) =>
			principal.HasClaim(c => c.Type == JwtRegisteredClaimNames.Aud) &&
			principal.IsInClientRole(principal.FindFirst(JwtRegisteredClaimNames.Aud)!.Value, roleName);
	}
}