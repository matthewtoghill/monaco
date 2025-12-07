using Serilog.Configuration;
using Serilog;
using Monaco.Template.Backend.Common.Serilog.Enrichers;

namespace Monaco.Template.Backend.Common.Serilog;

public static class SerilogExtensions
{
	extension(LoggerEnrichmentConfiguration enrichConfiguration)
	{
		public LoggerConfiguration WithOperationId()
		{
			ArgumentNullException.ThrowIfNull(enrichConfiguration, nameof(enrichConfiguration));

			return enrichConfiguration.With<OperationIdEnricher>();
		}
	}
}