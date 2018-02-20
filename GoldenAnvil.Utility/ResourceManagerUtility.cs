using System;
using System.Resources;

namespace GoldenAnvil.Utility
{
	public static class ResourceManagerUtility
	{
		public static string EnumToDisplayString<T>(this ResourceManager resources, T enumValue) where T : struct
		{
			if (!typeof(T).IsEnum)
				throw new ArgumentException(@"T must be an enumerated type.");

			var key = $"{typeof(T).Name}_{enumValue.ToString()}";
			return resources.GetString(key) ?? enumValue.ToString();
		}
	}
}
