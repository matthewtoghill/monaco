#if (massTransitIntegration)
using MassTransit;
#endif
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Monaco.Template.Backend.IntegrationTests.Factories;

public class WorkerServiceFactory : WebApplicationFactory<Worker.Program>
{
	private readonly AppFixture _fixture;

	public WorkerServiceFactory(AppFixture fixture)
	{
		_fixture = fixture;
	}

	protected override void ConfigureWebHost(IWebHostBuilder builder) =>
		builder.UseConfiguration(new ConfigurationManager
								 {
									 ["ConnectionStrings:AppDbContext"] = _fixture.SqlConnectionString,
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
			   .Configure(_ => { });

	public WebApplicationFactory<Worker.Program> GetCustomFactory(Action<IWebHostBuilder> configure) =>
		WithWebHostBuilder(configure);
}
#if (massTransitIntegration)

public static class WorkerServiceFactoryExtensions
{
	extension(IWebHostBuilder builder)
	{
		public IWebHostBuilder AddMassTransitTestHarnessForWorker() =>
			builder.ConfigureServices((context, services) =>
										  services.AddMassTransitTestHarness(cfg =>
																			 {
																				 var rabbitMqConfig = context.Configuration.GetSection("MessageBus:RabbitMQ");
																				 if (rabbitMqConfig.Exists())
																					 cfg.UsingRabbitMq((ctx, busCfg) =>
																									   {
																										   busCfg.Host(rabbitMqConfig["Host"],
																													   ushort.Parse(rabbitMqConfig["Port"] ?? "5672"),
																													   rabbitMqConfig["VHost"],
																													   h =>
																													   {
																														   h.Username(rabbitMqConfig["Username"]!);
																														   h.Password(rabbitMqConfig["Password"]!);
																													   });

																										   busCfg.ConfigureEndpoints(ctx, new DefaultEndpointNameFormatter(true));
																									   });
																			 }));
	}
}
#endif