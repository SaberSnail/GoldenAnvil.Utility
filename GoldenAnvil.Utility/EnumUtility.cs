using System;

namespace GoldenAnvil.Utility
{
	public static class EnumUtility
	{
		public static T[] Values<T>() where T : Enum => (T[]) Enum.GetValues(typeof(T));

		public static T ToEnum<T>(int value) where T : Enum
		{
			if (!Enum.IsDefined(typeof(T), value))
				throw new ArgumentException("Invalid enum value");
			return (T)Enum.ToObject(typeof(T), value);
		}
	}
}
