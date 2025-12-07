using FluentValidation;
using Monaco.Template.Backend.Common.Application.Commands;
using Monaco.Template.Backend.Common.Domain.Model;
using Monaco.Template.Backend.Common.Infrastructure.Context;
using Monaco.Template.Backend.Common.Infrastructure.Context.Extensions;
using System.Linq.Expressions;

namespace Monaco.Template.Backend.Common.Application.Validators.Extensions;

public static class ValidatorsExtensions
{
	public static readonly string ExistsRulesetName = "Exists";

	extension<TCommand>(AbstractValidator<TCommand> validator)
		where TCommand : CommandBase
	{
		public void CheckIfExists<TEntity>(BaseDbContext dbContext) where TEntity : Entity =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(x => x.Id)
																.MustExistAsync<TCommand, TEntity>(dbContext));

		public void CheckIfExists(Func<Guid, CancellationToken, Task<bool>> predicate) =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(x => x.Id)
																.MustAsync(predicate));

		public void CheckIfExists(Func<TCommand, Guid, CancellationToken, Task<bool>> predicate) =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(x => x.Id)
																.MustAsync(predicate));

		public void CheckIfExists<TPropertyType>(Expression<Func<TCommand, TPropertyType>> selector,
												 Func<TPropertyType, CancellationToken, Task<bool>> predicate) =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(selector)
																.MustAsync(predicate));

		public void CheckIfExists<TPropertyType>(Expression<Func<TCommand, TPropertyType>> selector,
												 Func<TCommand, TPropertyType, CancellationToken, Task<bool>> predicate) =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(selector)
																.MustAsync(predicate));

		public void CheckIfExists<TPropertyType>(Expression<Func<TCommand, TPropertyType>> selector,
												 Func<TCommand, TPropertyType, ValidationContext<TCommand>, CancellationToken, Task<bool>> predicate) =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(selector)
																.MustAsync(predicate));
	}

	extension<TCommand, TEntity, TResult>(AbstractValidator<TCommand> validator)
		where TCommand : CommandBase<TResult> 
		where TEntity : Entity
	{
		public void CheckIfExists(BaseDbContext dbContext) =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(x => x.Id)
																.MustExistAsync<TCommand, TEntity, TResult>(dbContext));
	}

	extension<TCommand, TPropertyType, TResult>(AbstractValidator<TCommand> validator)
		where TCommand : CommandBase<TResult>
	{
		public void CheckIfExists(Expression<Func<TCommand, TPropertyType>> selector,
								  Func<TPropertyType, CancellationToken, Task<bool>> predicate) =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(selector)
																.MustAsync(predicate));

		public void CheckIfExists(Expression<Func<TCommand, TPropertyType>> selector,
								  Func<TCommand, TPropertyType, CancellationToken, Task<bool>> predicate) =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(selector)
																.MustAsync(predicate));

		public void CheckIfExists(Expression<Func<TCommand, TPropertyType>> selector,
								  Func<TCommand,
									  TPropertyType,
									  ValidationContext<TCommand>,
									  CancellationToken,
									  Task<bool>> predicate) =>
			validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(selector)
																.MustAsync(predicate));
	}

	extension<TCommand>(IRuleBuilder<TCommand, Guid> ruleBuilder)
		where TCommand : CommandBase
	{
		public IRuleBuilderOptions<TCommand, Guid> MustExistAsync<TEntity>(BaseDbContext dbContext) where TEntity : Entity =>
			ruleBuilder.MustAsync(dbContext.ExistsAsync<TEntity>)
					   .WithMessage("The value {PropertyValue} is not valid");
	}

	extension<TCommand>(IRuleBuilder<TCommand, Guid?> ruleBuilder)
		where TCommand : CommandBase
	{
		public IRuleBuilderOptions<TCommand, Guid?> MustExistAsync<TEntity>(BaseDbContext dbContext) where TEntity : Entity =>
			ruleBuilder.MustAsync(async (id, ct) => id.HasValue &&
													await dbContext.ExistsAsync<TEntity>(x => x.Id == id.Value, ct))
					   .WithMessage("The value {PropertyValue} is not valid");
	}

	extension<TCommand, TEntity, TResult>(IRuleBuilder<TCommand, Guid?> ruleBuilder)
		where TCommand : CommandBase<TResult>
		where TEntity : Entity
	{
		public IRuleBuilderOptions<TCommand, Guid?> MustExistAsync(BaseDbContext dbContext) =>
			ruleBuilder.MustAsync(async (id, ct) => id.HasValue &&
													await dbContext.ExistsAsync<TEntity>(x => x.Id == id.Value, ct))
					   .WithMessage("The value {PropertyValue} is not valid");
	}

	extension<TCommand, TEntity, TResult>(IRuleBuilder<TCommand, Guid> ruleBuilder)
		where TCommand : CommandBase<TResult>
		where TEntity : Entity
	{
		public IRuleBuilderOptions<TCommand, Guid> MustExistAsync(BaseDbContext dbContext) =>
			ruleBuilder.MustAsync(dbContext.ExistsAsync<TEntity>)
					   .WithMessage("The value {PropertyValue} is not valid");
	}

	public static void CheckIfExists<TCommand, TResult>(this AbstractValidator<TCommand> validator,
														Func<Guid, CancellationToken, Task<bool>> predicate)
		where TCommand : CommandBase<TResult> =>
		validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(x => x.Id)
															.MustAsync(predicate));

	public static void CheckIfExists<TCommand, TResult>(this AbstractValidator<TCommand> validator,
														Func<TCommand, Guid, CancellationToken, Task<bool>> predicate)
		where TCommand : CommandBase<TResult> =>
		validator.RuleSet(ExistsRulesetName, () => validator.RuleFor(x => x.Id)
															.MustAsync(predicate));
}