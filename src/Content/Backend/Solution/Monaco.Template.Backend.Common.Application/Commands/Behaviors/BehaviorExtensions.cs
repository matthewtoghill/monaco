using LinqKit;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Monaco.Template.Backend.Common.Application.Commands.Behaviors;

public static class BehaviorExtensions
{
	private static readonly Type[]? CommandBaseDerivedTypes = null;
	private static readonly Type[]? IRequestDerivedTypes = null;

	/// <param name="services">The <see cref="IServiceCollection"/> to which the validation behaviors will be added.</param>
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Registers command validation behaviors for all command types derived from <c>CommandBase</c> or
		/// <c>CommandBase&lt;T&gt;</c> found in the specified assembly.
		/// </summary>
		/// <remarks>This method scans the provided assembly for types derived from <c>CommandBase</c> and
		/// <c>CommandBase&lt;T&gt;</c>. For each detected command type: <list type="bullet"> <item> <description>Scoped
		/// instances of <see cref="IPipelineBehavior{TRequest, TResponse}"/> are registered for validation existence checks
		/// using <c>CommandValidationExistsBehavior</c>.</description> </item> <item> <description>Scoped instances of <see
		/// cref="IPipelineBehavior{TRequest, TResponse}"/> are registered for validation logic using
		/// <c>CommandValidationBehavior</c>.</description> </item> </list></remarks>
		/// <param name="assembly">The assembly to scan for command types.</param>
		/// <returns>The updated <see cref="IServiceCollection"/> with the registered validation behaviors.</returns>
		public IServiceCollection RegisterCommandValidationBehaviors(Assembly assembly)
		{
			var allCommandTypes = GetCommandBaseDerivedTypes(assembly);

			allCommandTypes.ForEach(t =>
			{
				if (t.BaseType == typeof(CommandBase))
					services.AddScoped(typeof(IPipelineBehavior<,>).MakeGenericType(t, typeof(CommandResult)),
									   typeof(CommandValidationExistsBehavior<>).MakeGenericType(t))
							.AddScoped(typeof(IPipelineBehavior<,>).MakeGenericType(t, typeof(CommandResult)),
									   typeof(CommandValidationBehavior<>).MakeGenericType(t));
				else
				{
					var tResult = t.BaseType!.GenericTypeArguments.First();
					services.AddScoped(typeof(IPipelineBehavior<,>).MakeGenericType(t, typeof(CommandResult<>).MakeGenericType(tResult)),
									   typeof(CommandValidationExistsBehavior<,>).MakeGenericType(t, tResult))
							.AddScoped(typeof(IPipelineBehavior<,>).MakeGenericType(t, typeof(CommandResult<>).MakeGenericType(tResult)),
									   typeof(CommandValidationBehavior<,>).MakeGenericType(t, tResult));
				}
			});

			return services;
		}

		/// <summary>
		/// Registers pipeline behaviors to handle concurrency exceptions for command types in the specified assembly.
		/// </summary>
		/// <remarks>This method scans the provided assembly for types derived from <c>CommandBase</c> and
		/// <c>CommandBase&lt;TResult&gt;</c>. For each detected command type, it registers a corresponding scoped pipeline
		/// behavior to handle concurrency exceptions.</remarks>
		/// <param name="assembly">The assembly containing the command types to scan for concurrency exception behaviors.</param>
		/// <returns>The updated <see cref="IServiceCollection"/> with the registered behaviors.</returns>
		public IServiceCollection RegisterCommandConcurrencyExceptionBehaviors(Assembly assembly)
		{
			var iRequestDerivedTypes = GetIRequestDerivedTypes(assembly);

			iRequestDerivedTypes.ForEach(t =>
			{
				var requestInterface = t.GetInterfaces()
										.First(i => i.IsGenericType &&
													i.GetGenericTypeDefinition() == typeof(IRequest<>) &&
													(i.GenericTypeArguments[0] == typeof(CommandResult) ||
													 (i.GenericTypeArguments[0].IsGenericType &&
													  i.GenericTypeArguments[0].GetGenericTypeDefinition() == typeof(CommandResult<>))));

				var responseType = requestInterface.GenericTypeArguments[0];

				if (responseType == typeof(CommandResult))
					services.AddScoped(typeof(IPipelineBehavior<,>).MakeGenericType(t, typeof(CommandResult)),
									   typeof(ConcurrencyExceptionBehavior<>).MakeGenericType(t));
				else
				{
					var tResult = responseType.GenericTypeArguments[0];
					services.AddScoped(typeof(IPipelineBehavior<,>).MakeGenericType(t, typeof(CommandResult<>).MakeGenericType(tResult)),
									   typeof(ConcurrencyExceptionBehavior<,>).MakeGenericType(t, tResult));
				}
			});

			return services;
		}
	}


	private static Type[] GetCommandBaseDerivedTypes(Assembly assembly) =>
		CommandBaseDerivedTypes ??
		[
			.. assembly.GetTypes().Where(x => x.BaseType == typeof(CommandBase) ||
											  (x.BaseType?.IsGenericType == true &&
											   x.BaseType?.GetGenericTypeDefinition() == typeof(CommandBase<>)))
		];

	private static Type[] GetIRequestDerivedTypes(Assembly assembly) =>
		IRequestDerivedTypes ??
		[
			.. assembly.GetTypes().Where(x => x.GetInterfaces().Any(i =>
																		i.IsGenericType &&
																		i.GetGenericTypeDefinition() == typeof(IRequest<>) &&
																		(i.GenericTypeArguments[0] == typeof(CommandResult) ||
																		 (i.GenericTypeArguments[0].IsGenericType &&
																		  i.GenericTypeArguments[0].GetGenericTypeDefinition() == typeof(CommandResult<>)))))
		];
}