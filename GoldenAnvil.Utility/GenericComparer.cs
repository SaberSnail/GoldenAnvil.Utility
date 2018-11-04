using System;
using System.Collections.Generic;

namespace GoldenAnvil.Utility
{
	public sealed class GenericComparer<T> : IComparer<T>
	{
		public GenericComparer(Func<T, T, int> compareFunc)
		{
			if (compareFunc == null)
				throw new ArgumentNullException(nameof(compareFunc));

			m_compareFunc = compareFunc;
		}

		public int Compare(T left, T right)
		{
			return m_compareFunc(left, right);
		}

		readonly Func<T, T, int> m_compareFunc;
	}
}
