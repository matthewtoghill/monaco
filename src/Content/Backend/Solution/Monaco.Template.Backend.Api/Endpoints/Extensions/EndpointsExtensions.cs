using Asp.Versioning;

namespace Monaco.Template.Backend.Api.Endpoints.Extensions;

internal static class EndpointsExtensions
{
	extension(IEndpointRouteBuilder builder)
	{
		/// <summary>
		/// Registers all Minimal API endpoints 
		/// </summary>
		/// <returns></returns>
		public IEndpointRouteBuilder RegisterEndpoints()
		{
			var versionSet = builder.NewApiVersionSet()
									.HasApiVersion(new ApiVersion(1))
									.Build();

			return builder.AddCompanies(versionSet)
#if (filesSupport)
						  .AddCountries(versionSet)
						  .AddFiles(versionSet)
						  .AddProducts(versionSet);
#else
						  .AddCountries(versionSet);
#endif

		}
	}
}