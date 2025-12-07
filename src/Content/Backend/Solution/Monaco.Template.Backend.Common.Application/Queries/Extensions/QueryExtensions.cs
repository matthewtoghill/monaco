using Microsoft.EntityFrameworkCore;
using Monaco.Template.Backend.Common.Domain.Model;
using Monaco.Template.Backend.Common.Infrastructure.Context;
using Monaco.Template.Backend.Common.Infrastructure.Context.Extensions;
using System.Linq.Expressions;

namespace Monaco.Template.Backend.Common.Application.Queries.Extensions;

public static class QueryExtensions
{
	extension<T, TResult>(QueryBase<List<TResult>> request) where T : Entity
	{
		public async Task<List<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
														   Func<T, TResult> selector,
														   string defaultSortField,
														   Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
														   Dictionary<string, Expression<Func<T, object>>> mappedFieldsSort,
														   CancellationToken cancellationToken)
		{
			var result = await dbContext.Set<T>()
										.AsNoTracking()
										.ApplyFilter(request.QueryParams, mappedFieldsFilter)
										.ApplySort(request.Sort, defaultSortField, mappedFieldsSort)
										.ToListAsync(cancellationToken);
			return [.. result.Select(selector)];
		}
		
		public Task<List<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
													 Func<T, TResult> selector,
													 string defaultSortField,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
													 CancellationToken cancellationToken) =>
			request.ExecuteQueryAsync(dbContext,
									  selector,
									  defaultSortField,
									  mappedFieldsFilter,
									  mappedFieldsFilter,
									  cancellationToken);

		public async Task<List<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
														   Func<T, TResult> selector,
														   Func<IQueryable<T>, IQueryable<T>> queryFunc,
														   string defaultSortField,
														   Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
														   Dictionary<string, Expression<Func<T, object>>> mappedFieldsSort,
														   CancellationToken cancellationToken)
		{
			var query = dbContext.Set<T>().AsQueryable();
			query = queryFunc.Invoke(query);

			var result = await query.ApplyFilter(request.QueryParams, mappedFieldsFilter)
									.ApplySort(request.Sort, defaultSortField, mappedFieldsSort)
									.ToListAsync(cancellationToken);
			return [.. result.Select(selector)];
		}

		public Task<List<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
													 Func<T, TResult> selector,
													 Func<IQueryable<T>, IQueryable<T>> queryFunc,
													 string defaultSortField,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
													 CancellationToken cancellationToken) =>
			request.ExecuteQueryAsync(dbContext,
									  selector,
									  queryFunc,
									  defaultSortField,
									  mappedFieldsFilter,
									  mappedFieldsFilter,
									  cancellationToken);

		public async Task<List<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
														   Func<T, TResult> selector,
														   Func<QueryBase<List<TResult>>, Expression<Func<T, bool>>> expression,
														   string defaultSortField,
														   Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
														   Dictionary<string, Expression<Func<T, object>>> mappedFieldsSort,
														   CancellationToken cancellationToken)
		{
			var result = await dbContext.Set<T>()
										.AsNoTracking()
										.Where(expression.Invoke(request))
										.ApplyFilter(request.QueryParams, mappedFieldsFilter)
										.ApplySort(request.Sort, defaultSortField, mappedFieldsSort)
										.ToListAsync(cancellationToken);
			return [.. result.Select(selector)];
		}

		public Task<List<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
													 Func<T, TResult> selector,
													 Func<QueryBase<List<TResult>>, Expression<Func<T, bool>>> expression,
													 string defaultSortField,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
													 CancellationToken cancellationToken) =>
			request.ExecuteQueryAsync(dbContext,
									  selector,
									  expression,
									  defaultSortField,
									  mappedFieldsFilter,
									  mappedFieldsFilter,
									  cancellationToken);
	}

	extension<TReq, T, TResult>(TReq request) where TReq : QueryBase<List<TResult>> where T : Entity
	{
		public async Task<List<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
														   Func<T, TResult> selector,
														   Func<TReq, Expression<Func<T, bool>>> expression,
														   string defaultSortField,
														   Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
														   Dictionary<string, Expression<Func<T, object>>> mappedFieldsSort,
														   CancellationToken cancellationToken)
		{
			var result = await dbContext.Set<T>()
										.AsNoTracking()
										.Where(expression.Invoke(request))
										.ApplyFilter(request.QueryParams, mappedFieldsFilter)
										.ApplySort(request.Sort, defaultSortField, mappedFieldsSort)
										.ToListAsync(cancellationToken);
			return [.. result.Select(selector)];
		}
		
		public Task<List<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
													 Func<T, TResult> selector,
													 Func<TReq, Expression<Func<T, bool>>> expression,
													 string defaultSortField,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
													 CancellationToken cancellationToken) =>
			request.ExecuteQueryAsync(dbContext,
									  selector,
									  expression,
									  defaultSortField,
									  mappedFieldsFilter,
									  mappedFieldsFilter,
									  cancellationToken);
	}

	extension<T, TResult>(QueryPagedBase<TResult> request) where T : Entity
	{
		public Task<Page<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
													 Func<T, TResult> selector,
													 string defaultSortField,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsSort,
													 CancellationToken cancellationToken) =>
			dbContext.Set<T>()
					 .AsNoTracking()
					 .ApplyFilter(request.QueryParams, mappedFieldsFilter)
					 .ApplySort(request.Sort, defaultSortField, mappedFieldsSort)
					 .ToPageAsync(request.Offset, request.Limit, selector, cancellationToken);
		
		public Task<Page<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
													 Func<T, TResult> selector,
													 string defaultSortField,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
													 CancellationToken cancellationToken) =>
			request.ExecuteQueryAsync(dbContext,
									  selector,
									  defaultSortField,
									  mappedFieldsFilter,
									  mappedFieldsFilter,
									  cancellationToken);

		public Task<Page<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
													 Func<T, TResult> selector,
													 Func<QueryPagedBase<TResult>, Expression<Func<T, bool>>> expression,
													 string defaultSortField,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsSort,
													 CancellationToken cancellationToken) =>
			dbContext.Set<T>()
					 .AsNoTracking()
					 .Where(expression.Invoke(request))
					 .ApplyFilter(request.QueryParams, mappedFieldsFilter)
					 .ApplySort(request.Sort, defaultSortField, mappedFieldsSort)
					 .ToPageAsync(request.Offset, request.Limit, selector, cancellationToken);

		public Task<Page<TResult>> ExecuteQueryAsync(BaseDbContext dbContext,
													 Func<T, TResult> selector,
													 Func<QueryPagedBase<TResult>, Expression<Func<T, bool>>> expression,
													 string defaultSortField,
													 Dictionary<string, Expression<Func<T, object>>> mappedFieldsFilter,
													 CancellationToken cancellationToken) =>
			request.ExecuteQueryAsync(dbContext,
									  selector,
									  expression,
									  defaultSortField,
									  mappedFieldsFilter,
									  mappedFieldsFilter,
									  cancellationToken);
	}

	extension<T, TResult>(QueryByIdBase<TResult?> request) where T : Entity
	{
		public async Task<TResult?> ExecuteQueryAsync(BaseDbContext dbContext,
													  Func<T?, TResult?> selector,
													  CancellationToken cancellationToken)
		{
			var item = await dbContext.Set<T>()
									  .AsNoTracking()
									  .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
			return selector.Invoke(item);
		}
	}

	extension<TReq, T, TResult>(TReq request) where TReq : QueryByIdBase<TResult> where T : Entity
	{
		public async Task<TResult?> ExecuteQueryAsync(BaseDbContext dbContext,
													  Func<T?, TResult?> selector,
													  Func<TReq, Expression<Func<T, bool>>> expression,
													  CancellationToken cancellationToken)
		{
			var item = await dbContext.Set<T>()
									  .AsNoTracking()
									  .Where(expression.Invoke(request))
									  .SingleOrDefaultAsync(cancellationToken);
			return selector.Invoke(item);
		}
	}
}