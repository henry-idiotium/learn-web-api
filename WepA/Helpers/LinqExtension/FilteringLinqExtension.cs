using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WepA.Helpers.LinqExtension
{
	public static class FilteringLinqExtension
	{
		public static IQueryable<T> Filter<T>(this IQueryable<T> source,
			string filter, List<string> filterBy)
		{
			if (filter == string.Empty)
				return source;

			var result = new List<T>();
			foreach (var field in filterBy)
			{
				var thisProp = source.FirstOrDefault().GetType().GetProperties()
									 .FirstOrDefault(p => p.Name.ToLower() == field.ToLower())
									 .PropertyType;
				if (thisProp == typeof(DateTime) || thisProp == typeof(DateTime?)) continue;

				var parameter = Expression.Parameter(typeof(T), "p");
				var property = Expression.Property(parameter, field);
				var constant = Expression.Constant(filter);
				var method = field.GetType().GetMethod("Contains", new Type[] { typeof(string) });
				var call = Expression.Call(property, method, constant);
				var lambda = Expression.Lambda<Func<T, bool>>(call, parameter);

				foreach (var obj in source.Where(lambda))
				{
					if (!result.Contains(obj))
						result.Add(obj);
				}
			}
			return result.AsQueryable();
		}
	}
}

//region Old implementation
// var thisProp = source.FirstOrDefault().GetType().GetProperties()
// 									 .FirstOrDefault(p => p.Name.ToLower() == field.ToLower())
// 									 .PropertyType;
// if (thisProp == typeof(DateTime))
// {
// 	method = field.GetType().GetMethod("Equals", new Type[] { typeof(DateTime) });
// 	call = Expression.Call(
// 		Expression.Property(property, "Day"),
// 		method, Expression.Property(constant, "Day"));
// }
// var method = !(thisProp == typeof(DateTime) || thisProp == typeof(DateTime?)) ?
// 					field.GetType().GetMethod("Contains", new Type[] { typeof(string) }) :
// 					field.GetType().GetMethod("Equals", new Type[] { typeof(DateTime) });
// var call = !(thisProp == typeof(DateTime) || thisProp == typeof(DateTime?)) ?
// 					Expression.Call(property, method, constant) :
// 					Expression.Call(
// 						Expression.Property(property, "Day"),
// 						method, Expression.Property(constant, "Day"));
// var lambda = Expression.Lambda<Func<T, bool>>(call, parameter);
//endregion