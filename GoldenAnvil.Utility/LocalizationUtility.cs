using System.Globalization;

namespace GoldenAnvil.Utility
{
	public static class LocalizationUtility
	{
		public static string FormatCurrentUiCulture(this string format, params object[] args)
		{
			return string.Format(CultureInfo.CurrentUICulture, format, args);
		}
	}
}
