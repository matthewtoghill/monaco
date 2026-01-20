using Flurl.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
#if (workerService)
using Microsoft.Extensions.Hosting;
#endif
using System.Diagnostics.CodeAnalysis;
#if (massTransitIntegration)
using MassTransit.Testing;
#endif
#if (apiService && auth)
using Monaco.Template.Backend.IntegrationTests.Auth;
#endif

namespace Monaco.Template.Backend.IntegrationTests;

[ExcludeFromCodeCoverage]
public abstract class IntegrationTest : IAsyncLifetime
{
	protected readonly AppFixture Fixture;
#if (apiService)
	protected IFlurlClient Client;
#endif
#if (workerService)
	protected IHost WorkerServiceInstance;
#endif
#if (apiService && auth)
	protected KeycloakService? KeycloakService;
	protected AccessTokenDto? AccessToken;

	protected abstract bool RequiresAuthentication { get; }
#endif

	protected IntegrationTest(AppFixture fixture)
	{
		Fixture = fixture;

#if (apiService)
		var clientOptions = new WebApplicationFactoryClientOptions
							{
								AllowAutoRedirect = false
							};

		Client = new FlurlClient(Fixture.WebAppFactory.CreateClient(clientOptions))
#if (auth)
				 .AllowAnyHttpStatus()
				 .BeforeCall(call =>
							 {
								 if (AccessToken is not null)
									 call.Request.WithOAuthBearerToken(AccessToken.AccessToken);
							 });

		if (RequiresAuthentication)
			KeycloakService = new KeycloakService(Fixture.KeycloakContainer.GetBaseAddress(),
												  AppFixture.KeycloakRealm,
												  AppFixture.KeycloakRealmUsername,
												  AppFixture.KeycloakRealmPassword);
#else
				 .AllowAnyHttpStatus();

#endif
#endif
#if (workerService)
		WorkerServiceInstance = Fixture.WorkerServiceInstance;
#endif
	}

#if (apiService)
	protected IFlurlRequest CreateRequest(string endpoint) => Client.Request(endpoint);

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
		await Fixture.GetDbContext()
					 .Database
					 .ExecuteSqlRawAsync(await File.ReadAllTextAsync(filePath));

#if (apiService && massTransitIntegration)
	protected virtual ITestHarness GetApiTestHarness() =>
		Fixture.WebAppFactory
			   .Services
			   .GetTestHarness();

#endif
#if (workerService && massTransitIntegration)
	protected virtual ITestHarness GetServiceTestHarness() =>
		Fixture.WorkerServiceInstance
			   .Services
			   .GetTestHarness();

#endif
	public virtual async Task DisposeAsync() =>
		await Fixture.ResetDatabaseDataAsync();
}