using System;

namespace GoldenAnvil.Utility
{
	public static class ConversionUtility
	{
		public static T Convert<T>(object value)
		{
			var t = typeof(T);
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (value is null)
					return default(T);

				t = Nullable.GetUnderlyingType(t);
			}
			return (T) System.Convert.ChangeType(value, t);
		}
	}
}
