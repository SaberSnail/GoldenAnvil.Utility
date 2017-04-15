using System;

namespace GoldenAnvil.Utility
{
	public static class ObjectImpl
	{
		public static bool OperatorEquality<T>(T left, T right)
			where T : class, IEquatable<T>
		{
			if (ReferenceEquals(left, right))
				return true;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return false;
			return left.Equals(right);
		}

		public static bool OperatorInequality<T>(T left, T right)
			where T : class, IEquatable<T>
		{
			if (ReferenceEquals(left, right))
				return false;
			if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
				return true;
			return !left.Equals(right);
		}
	}
}
