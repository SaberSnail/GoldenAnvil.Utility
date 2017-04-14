using System.Collections.Generic;

namespace GoldenAnvil.Utility
{
	public static class EnumerableUtility
	{
		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}

		public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T value)
		{
			foreach (T item in source)
				yield return item;
			yield return value;
		}
	}
}
