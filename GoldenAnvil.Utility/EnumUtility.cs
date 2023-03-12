using System;

namespace GoldenAnvil.Utility
{
	public static class EnumUtility
	{
		public static T[] Values<T>() where T : struct => (T[]) Enum.GetValues(typeof(T));
	}
}
