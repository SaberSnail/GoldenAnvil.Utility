using System;
using System.Collections.Generic;

namespace GoldenAnvil.Utility
{
	public sealed class GenericComparer<T> : IComparer<T>
	{
		public GenericComparer(Func<T, T, int> compareFunc) =>
			m_compareFunc = compareFunc ?? throw new ArgumentNullException(nameof(compareFunc));

		public int Compare(T left, T right) => m_compareFunc(left, right);

		readonly Func<T, T, int> m_compareFunc;
	}
}
