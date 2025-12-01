using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Monaco.Template.Backend.Common.Api.Swagger;

/// <summary>
/// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
/// </summary>
/// <remarks>
/// This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
/// Once they are fixed and published, this class can be removed.
/// </remarks>
public class SwaggerDefaultValues : IOperationFilter
{
	/// <summary>
	/// Applies the filter to the specified operation using the given context.
	/// </summary>
	/// <param name="operation">The operation to apply the filter to.</param>
	/// <param name="context">The current operation filter context.</param>
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		var apiDescription = context.ApiDescription;
		operation.Deprecated |= apiDescription.IsDeprecated();

		if (operation.Parameters == null || operation.Parameters.Count == 0)
			return;

		// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
		// REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
		foreach (var parameter in operation.Parameters)
		{
			var description = apiDescription.ParameterDescriptions.FirstOrDefault(p => p.Name == parameter.Name);
			if (description is null)
				continue;

			parameter.Description ??= description.ModelMetadata?.Description;
		}
	}
}