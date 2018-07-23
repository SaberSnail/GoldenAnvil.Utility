using System.Collections.Generic;
using System.Linq;

namespace GoldenAnvil.Utility
{
	public static class EnumerableUtility
	{
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
		{
			return new HashSet<T>(items);
		}

		public static IEnumerable<T> Append<T>(this IEnumerable<T> items, T value)
		{
			foreach (T item in items)
				yield return item;
			yield return value;
		}

		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items)
		{
			return items ?? Enumerable.Empty<T>();
		}

		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> items)
		{
			return items.Where(x => x != null);
		}

		public static IEnumerable<T> Enumerate<T>(params T[] items)
		{
			return items;
		}
	}
}
