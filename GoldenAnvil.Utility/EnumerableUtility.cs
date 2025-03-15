using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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

		public static bool StartsWith<T>(this IEnumerable<T> items, IEnumerable<T> that) where T : class
		{
			var thatList = that.AsReadOnlyList();
			var itemsEnumerator = items.GetEnumerator();
			foreach (var thatItem in thatList)
			{
				if (!itemsEnumerator.MoveNext())
					return false;
				var thisItem = itemsEnumerator.Current;
				if (!thisItem.Equals(thatItem))
					return false;
			}
			return true;
		}

		public static bool AllMatchFirst<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> predicate)
		{
			bool isFirst = true;
			var value = default(TValue);
			foreach (var item in items)
			{
				if (isFirst)
				{
					value = predicate(item);
					isFirst = false;
				}
				else
				{
					if (!EqualityComparer<TValue>.Default.Equals(value, predicate(item)))
						return false;
				}
			}
			return !isFirst;
		}

		public static bool AllMatchFirstNotNull<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> predicate)
		{
			bool isFirst = true;
			var value = default(TValue);
			foreach (var item in items)
			{
				if (isFirst)
				{
					value = predicate(item);
					if (value is null)
						return false;
					isFirst = false;
				}
				else
				{
					if (!EqualityComparer<TValue>.Default.Equals(value, predicate(item)))
						return false;
				}
			}
			return !isFirst;
		}

		public static bool AllMatchFirstOrNull<TItem, TValue>(this IEnumerable<TItem> items, Func<TItem, TValue> predicate)
		{
			bool isFirst = true;
			var value = default(TValue);
			foreach (var item in items)
			{
				isFirst = false;
				var currentValue = predicate(item);
				if (value is null)
				{
					currentValue = value;
				}
				else if (currentValue is not null)
				{
					if (!EqualityComparer<TValue>.Default.Equals(value, predicate(item)))
						return false;
				}
			}
			return !isFirst;
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
