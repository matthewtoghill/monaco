using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Monaco.Template.Backend.Common.Api.Cors;

public static class CorsExtensions
{
	private const string DefaultCorsPoliciesSectionName = "CorsPolicies";
	private const string CorsDefaultPolicyName = "Default";
	private const string NameSection = "Name";
	private const string OriginsSection = "Origins";
	private const string MethodsSection = "Methods";
	private const string HeadersSection = "Headers";

	/// <param name="services"></param>
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Adds CORS policies configuration from the specified section name
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="sectionName"></param>
		/// <returns></returns>
		public IServiceCollection AddCorsPolicies(IConfiguration configuration,
												  string sectionName) =>
			services.AddCors(x =>
							 {
								 var corsConfigurations = configuration.GetSection(sectionName)
																	   .GetChildren()
																	   .ToList();

								 var defaultConfig = corsConfigurations.Find(c => c[NameSection] == CorsDefaultPolicyName);
								 if (defaultConfig is not null)
									 x.AddDefaultPolicy(ConfigurePolicy(defaultConfig));

								 corsConfigurations.ForEach(c => x.AddPolicy(c[NameSection]!, ConfigurePolicy(c)));
							 });

		/// <summary>
		/// Adds CORS policies configuration from the default section name (CorsPolicies)
		/// </summary>
		/// <param name="configuration"></param>
		/// <returns></returns>
		public IServiceCollection AddCorsPolicies(IConfiguration configuration) =>
			services.AddCorsPolicies(configuration, DefaultCorsPoliciesSectionName);
	}

	private static Action<CorsPolicyBuilder> ConfigurePolicy(IConfiguration config) =>
		p => p.WithOrigins(config.GetSection(OriginsSection)
								 .GetChildren()
								 .Select(o => o.Value!)
								 .ToArray())
			  .WithMethods(config.GetSection(MethodsSection)
								 .GetChildren()
								 .Select(o => o.Value!)
								 .ToArray())
			  .WithHeaders(config.GetSection(HeadersSection)
								 .GetChildren()
								 .Select(o => o.Value!)
								 .ToArray());
}