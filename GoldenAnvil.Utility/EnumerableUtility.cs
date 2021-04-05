using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GoldenAnvil.Utility
{
	public static class EnumerableUtility
	{
		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> items) => items ?? Enumerable.Empty<T>();

		public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> items) => items.Where(x => x != null);

		public static IEnumerable<T> Enumerate<T>(params T[] items) => items;

		public static IReadOnlyList<T> AsReadOnlyList<T>(this IEnumerable<T> items)
		{
			return items as IReadOnlyList<T> ??
				(items is IList<T> list ? (IReadOnlyList<T>) new ReadOnlyListAdapter<T>(list) : items.ToList().AsReadOnly());
		}

		public static T FirstOrDefault<T>(this IEnumerable<T> items, T defaultValue)
		{
			if (items is null)
				throw new ArgumentNullException(nameof(items));

			foreach (var item in items)
				return item;

			return defaultValue;
		}

		public static T FirstOrDefault<T>(this IEnumerable<T> items, Func<T, bool> predicate, T defaultValue)
		{
			if (items is null)
				throw new ArgumentNullException(nameof(items));
			if (predicate is null)
				throw new ArgumentNullException(nameof(predicate));

			foreach (var item in items)
			{
				if (predicate(item))
					return item;
			}

			return defaultValue;
		}

		private sealed class ReadOnlyListAdapter<T> : IReadOnlyList<T>
		{
			public ReadOnlyListAdapter(IList<T> list) => m_list = list ?? throw new ArgumentNullException(nameof(list));

			public int Count => m_list.Count;
			public T this[int index] => m_list[index];
			public IEnumerator<T> GetEnumerator() => m_list.GetEnumerator();
			IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) m_list).GetEnumerator();

			readonly IList<T> m_list;
		}
	}
}
