using System.Linq.Expressions;

namespace Monaco.Template.Backend.Common.Infrastructure.Context.Extensions;

public static class SortingExtensions
{
	extension<T>(IQueryable<T> source)
	{
		public IQueryable<T> ApplySort(string?[] sortFields,
									   string defaultSortField,
									   Dictionary<string, Expression<Func<T, object>>> sortMap)
		{
			ArgumentException.ThrowIfNullOrEmpty(defaultSortField);

			var (sortMapLower, lstSort) = GetData(sortFields, defaultSortField, sortMap);

			var query = source.AsQueryable();
			foreach (var (key, value) in lstSort) // Loop through the fields and apply the sorting
				query = query.GetOrderedQuery(sortMapLower[key], value, key == lstSort.Keys.First());
			return query;
		}

		private IOrderedQueryable<T> GetOrderedQuery(Expression<Func<T, object>> expression, bool ascending, bool firstSort)
		{
			var bodyExpression = (MemberExpression)(expression.Body.NodeType == ExpressionType.Convert ? ((UnaryExpression)expression.Body).Operand : expression.Body);
			var sortLambda = Expression.Lambda(bodyExpression, expression.Parameters);
			Expression<Func<IOrderedQueryable<T>>> sortMethod = firstSort
																	? ascending
																		  ? () => source.OrderBy<T, object>(k => null!)
																		  : () => source.OrderByDescending<T, object>(k => null!)
																	: ascending
																		? () => ((IOrderedQueryable<T>)source).ThenBy<T, object>(k => null!)
																		: () => ((IOrderedQueryable<T>)source).ThenByDescending<T, object>(k => null!);

			var methodCallExpression = (MethodCallExpression)sortMethod.Body;
			var method = methodCallExpression.Method.GetGenericMethodDefinition();
			var genericSortMethod = method.MakeGenericMethod(typeof(T), bodyExpression.Type);
			return (IOrderedQueryable<T>)genericSortMethod.Invoke(source, [source, sortLambda])!;
		}
	}

	extension<T>(IEnumerable<T> source)
	{
		public IEnumerable<T> ApplySort(string?[] sortFields,
										string defaultSortField,
										Dictionary<string, Expression<Func<T, object>>> sortMap)
		{
			ArgumentException.ThrowIfNullOrEmpty(defaultSortField);

			var (sortMapLower, lstSort) = GetData(sortFields, defaultSortField, sortMap);

			var query = source.AsEnumerable();
			foreach (var (key, value) in lstSort) // Loop through the fields and apply the sorting
				query = query.GetOrderedQuery(sortMapLower[key], value, key == lstSort.Keys.First());
			return query;
		}

		private IOrderedEnumerable<T> GetOrderedQuery(Expression<Func<T, object>> expression, bool ascending, bool firstSort)
		{
			var bodyExpression = (MemberExpression)(expression.Body.NodeType == ExpressionType.Convert ? ((UnaryExpression)expression.Body).Operand : expression.Body);
			var sortLambda = Expression.Lambda(bodyExpression, expression.Parameters);
			Expression<Func<IOrderedEnumerable<T>>> sortMethod = firstSort
																	 ? ascending
																		   ? () => source.OrderBy<T, object>(k => null!)
																		   : () => source.OrderByDescending<T, object>(k => null!)
																	 : ascending
																		 ? () => ((IOrderedEnumerable<T>)source).ThenBy<T, object>(k => null!)
																		 : () => ((IOrderedEnumerable<T>)source).ThenByDescending<T, object>(k => null!);
			if (sortMethod.Body is not MethodCallExpression methodCallExpression)
				throw new Exception("oops");

			var meth = methodCallExpression.Method.GetGenericMethodDefinition();
			var genericSortMethod = meth.MakeGenericMethod(typeof(T), bodyExpression.Type);
			return (IOrderedEnumerable<T>)genericSortMethod.Invoke(source, [source, sortLambda.Compile()])!;
		}
	}

	public static (Dictionary<string, Expression<Func<T, object>>> sortMapLower, Dictionary<string, bool> lstSort)
		GetData<T>(IEnumerable<string?> sortFields, string defaultSortField, Dictionary<string, Expression<Func<T, object>>> sortMap)
	{
		// convert a Dictionary with Keys into lowercase to ease searching
		var sortMapLower = sortMap.ToDictionary(x => x.Key.ToLower(), x => x.Value);
		// convert the list of fields to sort into a dictionary field/direction and filter out the non-existing ones
		var lstSort = ProcessSortParam(sortFields, sortMapLower);
		if (lstSort.Count == 0) // if there's none remaining, load the default ones
			lstSort = ProcessSortParam([defaultSortField], sortMapLower);

		return (sortMapLower, lstSort);
	}

	private static Dictionary<string, bool> ProcessSortParam<T>(IEnumerable<string?> sortFields,
																Dictionary<string, Expression<Func<T, object>>> sortMap) =>
		sortFields.Where(x => x is not null)
				  .ToDictionary(x => (x is ['-', .. var param] ? param : x!).ToLower(),
								x => x is not ['-', ..])
				  .Where(x => sortMap.ContainsKey(x.Key))
				  .ToDictionary(x => x.Key, x => x.Value);
}