using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GoldenAnvil.Utility
{
	public static class TitleUtility
	{
		public static string GetUniqueTitle(string title, HashSet<string> existingTitles)
		{
			if (existingTitles == null)
				throw new ArgumentNullException(nameof(existingTitles));

			if (!existingTitles.Contains(title))
				return title;

			var baseTitleRegEx = new Regex("(.*) \\((\\d+)\\)$");
			var result = baseTitleRegEx.Match(title);
			var baseTitle = result.Success ? result.Groups[1].Value : title;
			string bestTitle;
			int index = result.Success ? StringUtility.TryParse<int>(result.Groups[2].Value) ?? 1 : 1;
			do
			{
				bestTitle = $"{baseTitle} ({index})";
				index++;
			}
			while (existingTitles.Contains(bestTitle));

			return bestTitle;
		}
	}
}
