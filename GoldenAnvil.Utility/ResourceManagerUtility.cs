using System;
using System.Resources;

namespace GoldenAnvil.Utility
{
	public static class ResourceManagerUtility
	{
		public static string EnumToDisplayString<T>(this ResourceManager resources, T enumValue) where T : Enum
		{
			var key = $"{typeof(T).Name}_{enumValue.ToString()}";
			return resources.GetString(key) ?? enumValue.ToString();
		}

		public static string EnumToDisplayString<T>(this ResourceManager resources, T enumValue, string modifier) where T : Enum
		{
			var key = $"{typeof(T).Name}_{enumValue.ToString()}_{modifier}";
			return resources.GetString(key) ?? enumValue.ToString();
		}
	}
}
