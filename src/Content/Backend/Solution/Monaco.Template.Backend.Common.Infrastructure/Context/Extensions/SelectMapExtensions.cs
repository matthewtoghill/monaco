using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Monaco.Template.Backend.Common.Infrastructure.Context.Extensions;

public static class SelectMapExtensions
{
	extension<T>(IQueryable<T> source)
	{
		public async Task<TDto?> SingleOrDefaultMapAsync<TDto>(Expression<Func<T, bool>> predicate,
															   Expression<Func<T, TDto>> selector,
															   CancellationToken cancellationToken = default) =>
			await source.Where(predicate)
						.Select(selector)
						.DecompileAsync()
						.SingleOrDefaultAsync(cancellationToken);

		public async Task<TDto?> SingleOrDefaultMapAsync<TDto>(Expression<Func<T, TDto>> selector,
															   CancellationToken cancellationToken = default) =>
			await source.Select(selector)
						.DecompileAsync()
						.SingleOrDefaultAsync(cancellationToken);

		public async Task<TDto?> FirstOrDefaultMapAsync<TDto>(Expression<Func<T, bool>> predicate,
															  Expression<Func<T, TDto>> selector,
															  CancellationToken cancellationToken = default) =>
			await source.Where(predicate)
						.Select(selector)
						.DecompileAsync()
						.FirstOrDefaultAsync(cancellationToken);

		public async Task<TDto?> FirstOrDefaultMapAsync<TDto>(Expression<Func<T, TDto>> selector,
															  CancellationToken cancellationToken = default) =>
			await source.Select(selector)
						.DecompileAsync()
						.FirstOrDefaultAsync(cancellationToken);

		public async Task<TDto?> SingleOrDefaultAsync<TDto>(Expression<Func<T, bool>> predicate,
															Expression<Func<T, TDto>> selector,
															CancellationToken cancellationToken = default) =>
			await source.Where(predicate)
						.Select(selector)
						.SingleOrDefaultAsync(cancellationToken);

		public async Task<TDto?> FirstOrDefaultAsync<TDto>(Expression<Func<T, bool>> predicate,
														   Expression<Func<T, TDto>> selector,
														   CancellationToken cancellationToken = default) =>
			await source.Where(predicate)
						.Select(selector)
						.FirstOrDefaultAsync(cancellationToken);

		public async Task<List<TDto>> ToListMapAsync<TDto>(Expression<Func<T, TDto>> selector,
														   CancellationToken cancellationToken = default) =>
			await source.Select(selector)
						.DecompileAsync()
						.ToListAsync(cancellationToken);
	}
}