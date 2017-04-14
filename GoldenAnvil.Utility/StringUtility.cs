using System.Globalization;

namespace GoldenAnvil.Utility
{
	public static class StringUtility
	{
		public static string FormatInvariant(this string format, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}
	}
}
