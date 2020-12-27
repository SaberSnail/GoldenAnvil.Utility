using System;
using System.Collections.Generic;

namespace GoldenAnvil.Utility
{
	public sealed class GenericEqualityComparer<T> : IEqualityComparer<T>
	{
		public GenericEqualityComparer(Func<T, T, bool> compareFunc, Func<T, int> hashCodeFunc)
		{
			m_compareFunc = compareFunc ?? throw new ArgumentNullException(nameof(compareFunc));
			m_hashCodeFunc = hashCodeFunc ?? throw new ArgumentNullException(nameof(hashCodeFunc));
		}

		public bool Equals(T left, T right) => m_compareFunc(left, right);

		public int GetHashCode(T obj) => m_hashCodeFunc(obj);

		readonly Func<T, T, bool> m_compareFunc;
		readonly Func<T, int> m_hashCodeFunc;
	}
}
