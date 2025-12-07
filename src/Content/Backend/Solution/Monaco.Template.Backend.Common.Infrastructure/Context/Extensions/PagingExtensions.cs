using Microsoft.EntityFrameworkCore;
using Monaco.Template.Backend.Common.Domain.Model;
using System.Linq.Expressions;

namespace Monaco.Template.Backend.Common.Infrastructure.Context.Extensions;

public static class PagingExtensions
{
	extension<T>(IQueryable<T> query)
	{
		public async Task<Page<TResult>> ToPageAsync<TResult>(int offset,
															  int limit,
															  Func<T, TResult> selector,
															  CancellationToken cancellationToken = default)
		{
			var results = await query.Select(x => new
												  {
													  Item = x,
													  TotalCount = query.Count()
												  })
									 .Skip(offset)
									 .Take(limit)
									 .ToListAsync(cancellationToken);
			return new(results.Select(x => selector(x.Item)),
					   offset,
					   limit,
					   results.FirstOrDefault()?.TotalCount ?? 0);
		}

		public async Task<Page<TResult>> ToPageAsync<TKey, TResult>(int offset,
																	int limit,
																	Func<T, TResult> selector,
																	Expression<Func<T, TKey>>? orderBy,
																	CancellationToken cancellationToken = default) =>
			await (orderBy is null
					   ? query
					   : query.OrderBy(orderBy)).ToPageAsync(offset,
															 limit,
															 selector,
															 cancellationToken);

		public async Task<Page<TResult>> ToPageAsync<TKey, TSec, TResult>(int offset,
																		  int limit,
																		  Func<T, TResult> selector,
																		  Expression<Func<T, TKey>>? orderBy,
																		  Expression<Func<T, TSec>>? thenBy,
																		  CancellationToken cancellationToken = default) =>
			await (orderBy is null
					   ? query
					   : thenBy is null
						   ? query.OrderBy(orderBy)
						   : query.OrderBy(orderBy)
								  .ThenBy(thenBy)).ToPageAsync(offset,
															   limit,
															   selector,
															   cancellationToken);
	}

	extension<T>(IEnumerable<T> enumerable)
	{
		public Page<TResult> ToPage<TResult>(int offset,
											 int limit,
											 Func<T, TResult> selector)
		{
			var list = enumerable.ToList();
			return new(list.Skip(offset)
						   .Take(limit)
						   .Select(selector),
					   offset,
					   limit,
					   list.Count);
		}
	}
}