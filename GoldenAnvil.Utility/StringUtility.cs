using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

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

		public static string Join(this IEnumerable<string> parts, char joinChar)
		{
			var builder = new StringBuilder();
			foreach (var part in parts)
			{
				if (builder.Length != 0)
					builder.Append(joinChar);
				builder.Append(part);
			}

			return builder.ToString();
		}

		public static bool Contains(this string value, string check, StringComparison comparison) =>
			value.IndexOf(check, comparison) >= 0;

		public static IReadOnlyList<string> GetWordsFromCamelCase(string input)
		{
			return Regex.Matches(input, @"(^[\p{Ll}]+|[\p{Lu}\p{N}]+(?![\p{Ll}])|\p{P}?[\p{Lu}][\p{Ll}]+)")
				.OfType<Match>()
				.Select(m => m.Value)
				.AsReadOnlyList();
		}

		public static T? TryParse<T>(string value, IFormatProvider formatProvider = null)
		where T : struct, IParsable<T>
		{
			var format = formatProvider ?? CultureInfo.CurrentCulture;
			if (T.TryParse(value, format, out var result))
				return result;
			return null;
		}
	}
}
