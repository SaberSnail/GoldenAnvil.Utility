using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace GoldenAnvil.Utility
{
	public static class StringUtility
	{
		public static string FormatInvariant(this string format, params object[] args)
		{
			return string.Format(CultureInfo.InvariantCulture, format, args);
		}

		public static string Join(this IEnumerable<string> parts, string joinText)
		{
			var builder = new StringBuilder();
			foreach (var part in parts)
			{
				if (builder.Length != 0)
					builder.Append(joinText);
				builder.Append(part);
			}

			return builder.ToString();
		}

		public static bool Contains(this string value, string check, StringComparison comparison) =>
			value.IndexOf(check, comparison) >= 0;
	}
}
