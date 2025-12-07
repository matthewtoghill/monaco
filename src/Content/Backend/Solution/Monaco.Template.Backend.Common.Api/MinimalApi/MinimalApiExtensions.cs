using Asp.Versioning.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Monaco.Template.Backend.Common.Api.MinimalApi;

public static class MinimalApiExtensions
{
	extension(IEndpointRouteBuilder builder)
	{
		public RouteGroupBuilder CreateApiGroupBuilder(ApiVersionSet versionSet,
													   string collectionName,
													   int version = 1) =>
			builder.MapGroup(string.Concat("api/v{apiVersion:apiVersion}/", collectionName))
				   .WithName(collectionName)
				   .WithDisplayName(collectionName)
				   .WithTags(collectionName)
				   .WithApiVersionSet(versionSet)
#if (!auth)
				   .HasApiVersion(version);
#else
			   .HasApiVersion(version)
			   .RequireAuthorization();
#endif

		public RouteHandlerBuilder MapGet(string pattern,
										  Delegate handler,
										  string name,
										  string summary) =>
			builder.MapGet(pattern,
						   handler,
						   name,
						   summary,
						   string.Empty);

		public RouteHandlerBuilder MapGet(string pattern,
										  Delegate handler,
										  string name,
										  string summary,
										  string description) =>
			builder.MapGet(pattern,
						   handler)
				   .WithName(name)
				   .WithSummary(summary)
				   .WithDescription(description);

		public RouteHandlerBuilder MapPost(string pattern,
										   Delegate handler,
										   string name,
										   string summary) =>
			builder.MapPost(pattern,
							handler,
							name,
							summary,
							string.Empty);

		public RouteHandlerBuilder MapPost(string pattern,
										   Delegate handler,
										   string name,
										   string summary,
										   string description) =>
			builder.MapPost(pattern,
							handler)
				   .WithName(name)
				   .WithSummary(summary)
				   .WithDescription(description);

		public RouteHandlerBuilder MapPut(string pattern,
										  Delegate handler,
										  string name,
										  string summary) =>
			builder.MapPut(pattern,
						   handler,
						   name,
						   summary,
						   string.Empty);

		public RouteHandlerBuilder MapPut(string pattern,
										  Delegate handler,
										  string name,
										  string summary,
										  string description) =>
			builder.MapPut(pattern,
						   handler)
				   .WithName(name)
				   .WithSummary(summary)
				   .WithDescription(description);

		public RouteHandlerBuilder MapDelete(string pattern,
											 Delegate handler,
											 string name,
											 string summary) =>
			builder.MapDelete(pattern,
							  handler,
							  name,
							  summary,
							  string.Empty);

		public RouteHandlerBuilder MapDelete(string pattern,
											 Delegate handler,
											 string name,
											 string summary,
											 string description) =>
			builder.MapDelete(pattern,
							  handler)
				   .WithName(name)
				   .WithSummary(summary)
				   .WithDescription(description);
	}
}