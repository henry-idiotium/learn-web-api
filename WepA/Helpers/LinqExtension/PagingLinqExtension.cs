using System.Linq;

namespace WepA.Helpers.LinqExtension
{
	public static class PagingLinqExtension
	{
		public static IQueryable<T> Paginate<T>(this IQueryable<T> source, int page, int pageSize)
		{
			return source.Skip(page * pageSize).Take(pageSize);
		}
	}
}