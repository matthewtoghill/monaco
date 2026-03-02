using Flurl.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
#if (apiService && auth)
using Monaco.Template.Backend.IntegrationTests.Auth;
#endif

namespace Monaco.Template.Backend.IntegrationTests;

[ExcludeFromCodeCoverage]
public abstract class IntegrationTest : IAsyncLifetime
{
	protected readonly AppFixture Fixture;
#if (apiService && auth)
	protected KeycloakService? KeycloakService;
	protected AccessTokenDto? AccessToken;

	protected abstract bool RequiresAuthentication { get; }
#endif

	protected IntegrationTest(AppFixture fixture)
	{
		Fixture = fixture;

#if (apiService)
#if (auth)

		if (RequiresAuthentication)
			KeycloakService = new KeycloakService(Fixture.KeycloakContainer.GetBaseAddress(),
												  AppFixture.KeycloakRealm,
												  AppFixture.KeycloakRealmUsername,
												  AppFixture.KeycloakRealmPassword);
#else
				 .AllowAnyHttpStatus();

#endif
#endif
	}

#if (apiService)
	protected FlurlClient GetClient(WebApplicationFactory<Api.Program> factory) =>
        new FlurlClient(factory.CreateClient(new() { AllowAutoRedirect = false }))
#if (auth)
            .AllowAnyHttpStatus()
            .BeforeCall(call =>
            {
                if (AccessToken is not null)
                    call.Request.WithOAuthBearerToken(AccessToken.AccessToken);
            });

#else
			.AllowAnyHttpStatus();

#endif
#endif
	public virtual Task InitializeAsync() =>
		Task.CompletedTask;

#if (apiService && auth)
	protected virtual async Task SetupAccessToken(string audienceClientId,
												  string[] roles,
												  string[] scopes)
	{
		if (!RequiresAuthentication)
			return;

		var client = await KeycloakService!.CreateTestClient(audienceClientId, roles, scopes);
		AccessToken = await KeycloakService.GetAccessToken(client);
	}

	protected virtual Task SetupAccessToken(string[] roles) =>
		SetupAccessToken(Auth.Auth.AudienceClientId,
						 roles,
						 Auth.Auth.Scopes);

#endif

	protected virtual async Task RunScriptAsync(string filePath) =>
		await Fixture.GetDbContext(Fixture.WebAppFactory.Services)
					 .Database
					 .ExecuteSqlRawAsync(await File.ReadAllTextAsync(filePath));

	public virtual async Task DisposeAsync() =>
		await Fixture.ResetDatabaseDataAsync();
}