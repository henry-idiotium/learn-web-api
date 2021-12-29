using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WepA.Helpers.LinqExtension
{
	public static class OrderingLinqExtension
	{
		static IOrderedQueryable<T> OrderingHelper<T>(this IQueryable<T> source,
			string field, bool ascending, bool anotherLevel)
		{
			var parameter = Expression.Parameter(typeof(T), "p");
			var property = Expression.PropertyOrField(parameter, field);
			var sort = Expression.Lambda(property, parameter);

			var call = Expression.Call(
				typeof(IQueryable),
				(anotherLevel ? "ThenBy" : "OrderBy") + (ascending ? string.Empty : "descending"),
				new Type[] { typeof(T), property.Type },
				source.Expression,
				Expression.Quote(sort));

			return (IOrderedQueryable<T>)source.Provider.CreateQuery<T>(call);
		}

		public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source,
			string field, bool descending = false)
		{
			return OrderingHelper(source, field, descending, false);
		}

		public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source,
			string field, bool descending = false)
		{
			return OrderingHelper(source, field, descending, true);
		}

		public static IQueryable<T> OrderByMultiple<T>(this IQueryable<T> source,
			List<string> fields)
		{
			if (fields == null || !fields.Any())
				return source;

			IOrderedQueryable<T> orderedSource;
			if (fields.First()[..1] == "-")
				orderedSource = source.OrderBy(fields.First()[1..], true);
			else
				orderedSource = source.OrderBy(fields.First());

			if (fields.Count > 1)
			{
				foreach (var field in fields.Skip(1))
				{
					if (field[..1] == "-")
						orderedSource = orderedSource.ThenBy(field[1..], true);
					else
						orderedSource = orderedSource.ThenBy(field);
				}
			}
			return orderedSource;
		}
	}
}