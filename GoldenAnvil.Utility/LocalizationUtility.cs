using System.Globalization;

namespace GoldenAnvil.Utility
{
	public static class LocalizationUtility
	{
		public static string FormatCurrentCulture(this string format, params object[] args)
		{
			return string.Format(CultureInfo.CurrentCulture, format, args);
		}
	}
}
