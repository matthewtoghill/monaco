#if (massTransitIntegration)
using MassTransit;
#endif
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Monaco.Template.Backend.IntegrationTests.Factories;

public sealed class ApiWebApplicationFactory : WebApplicationFactory<Api.Program>
{
	private readonly AppFixture _fixture;

	public ApiWebApplicationFactory(AppFixture fixture)
	{
		_fixture = fixture;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder) =>
		builder.UseConfiguration(new ConfigurationManager
								 {
									 ["ConnectionStrings:AppDbContext"] = _fixture.SqlConnectionString,
#if (auth)
									 ["SSO:Authority"] = _fixture.KeycloakRealmUrl,
#endif
#if (filesSupport)
									 ["BlobStorage:ConnectionString"] = _fixture.StorageConnectionString,
#endif
#if (massTransitIntegration)
									 ["MessageBus:RabbitMQ:Host"] = _fixture.RabbitMqHost,
									 ["MessageBus:RabbitMQ:Port"] = _fixture.RabbitMqPort.ToString(),
									 ["MessageBus:RabbitMQ:Username"] = _fixture.RabbitMqUsername,
									 ["MessageBus:RabbitMQ:Password"] = _fixture.RabbitMqPassword
#endif
								 })
			   .UseSetting("https_port", "8080");

	public WebApplicationFactory<Api.Program> GetCustomFactory(Action<IWebHostBuilder> configure) =>
		WithWebHostBuilder(configure);
}
#if (massTransitIntegration)

public static class WebAppFactoryExtensions
{
	extension(IWebHostBuilder builder)
	{
		public IWebHostBuilder AddMassTransitTestHarnessForWebApp() =>
			builder.ConfigureServices((context, services) =>
										  services.AddMassTransitTestHarness(cfg =>
																			 {
																				 var rabbitMqConfig = context.Configuration.GetSection("MessageBus:RabbitMQ");
																				 if (rabbitMqConfig.Exists())
																					 cfg.UsingRabbitMq((_, busCfg) => busCfg.Host(rabbitMqConfig["Host"],
																																  ushort.Parse(rabbitMqConfig["Port"] ?? "5672"),
																																  rabbitMqConfig["VHost"],
																																  h =>
																																  {
																																	  h.Username(rabbitMqConfig["Username"]!);
																																	  h.Password(rabbitMqConfig["Password"]!);
																																  }));
																			 }));
	}
}
#endif