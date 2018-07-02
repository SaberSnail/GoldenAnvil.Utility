using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace GoldenAnvil.Utility
{
	/// <summary>
	/// This class enables the localization of pluralizable strings. This is based on the data available at
	/// http://cldr.unicode.org/index/cldr-spec/plural-rules
	/// </summary>
	public static class PluralizationUtility
	{
		public static string Pluralize(this ResourceManager resources, string baseKey, int value)
		{
			var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
			var pattern = s_patterns.GetValueOrDefault(language);
			if (pattern == null)
				throw new InvalidOperationException($"Unsupported pluralization language ({language}) in {nameof(PluralizationUtility)}.");
			return resources.GetString(baseKey + pattern.GetCardinalRuleName(value));
		}

		private interface IPluralizationPattern
		{
			string GetCardinalRuleName(int value);
		}

		private class EnglishPattern : IPluralizationPattern
		{
			public string GetCardinalRuleName(int value)
			{
				if (value == 1)
					return c_one;
				return c_other;
			}
		}

		const string c_one = "_One";
		const string c_other = "_Other";

		static Dictionary<string, IPluralizationPattern> s_patterns = new Dictionary<string, IPluralizationPattern>
		{
			{ "en", new EnglishPattern() },
		};
	}
}
