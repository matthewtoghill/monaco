using Microsoft.EntityFrameworkCore;
using Monaco.Template.Backend.Common.Domain.Model;
using System.Linq.Expressions;

namespace Monaco.Template.Backend.Common.Infrastructure.Context.Extensions;

public static class OperationsExtensions
{
	extension(DbContext dbContext)
	{
		public Task<bool> ExistsAsync<T>(Guid id,
										 CancellationToken cancellationToken) where T : Entity =>
			dbContext.Set<T>().AnyAsync(x => x.Id == id, cancellationToken);

		public Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> predicate,
										 CancellationToken cancellationToken) where T : class =>
			dbContext.Set<T>().AnyAsync(predicate, cancellationToken);

		public async Task<T?> GetAsync<T>(Guid? id,
										  CancellationToken cancellationToken) where T : class =>
			id.HasValue
				? await dbContext.GetAsync<T>(id.Value, cancellationToken)
				: null;

		public async Task<T> GetAsync<T>(Guid id,
										 CancellationToken cancellationToken) where T : class =>
			(await dbContext.Set<T>().FindAsync([id], cancellationToken))!;

		public IQueryable<TResult> Set<TResult>(Type t) =>
			(IQueryable<TResult>)dbContext.GetType()
										  .GetMethod("Set", Type.EmptyTypes)?
										  .MakeGenericMethod(t)
										  .Invoke(dbContext, [])!;

		public async Task<List<T>> GetListByIdsAsync<T>(List<Guid> items,
														CancellationToken cancellationToken) where T : Entity =>
			items.Count > 0
				? await dbContext.Set<T>()
								 .Where(x => items.Contains(x.Id))
								 .ToListAsync(cancellationToken)
				: [];
	}
}