#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;

namespace GoldenAnvil.Utility;

public sealed class TitleComparer : IComparer<string>
{
	public TitleComparer(StringComparison comparison)
	{
		m_comparison = comparison;
	}

	public int Compare(string? left, string? right)
	{
		if (left is null && right is null)
			return 0;
		if (left is null)
			return -1;
		if (right is null)
			return 1;

		var leftSpan = left.AsSpan();
		var rightSpan = right.AsSpan();

		var suffixLength = GetCommonNonDigitSuffix(leftSpan, rightSpan);
		if (suffixLength == leftSpan.Length && suffixLength == rightSpan.Length)
			return 0;
		if (suffixLength == leftSpan.Length)
			return -1;
		if (suffixLength == rightSpan.Length)
			return 1;

		leftSpan = leftSpan.Slice(0, leftSpan.Length - suffixLength);
		rightSpan = rightSpan.Slice(0, rightSpan.Length - suffixLength);

		var commonPrefixLength = leftSpan.CommonPrefixLength(rightSpan);
		var leftDigitSpan = GetDigitSpan(leftSpan, commonPrefixLength);
		var rightDigitSpan = GetDigitSpan(rightSpan, commonPrefixLength);

		if (!leftDigitSpan.IsEmpty && !rightDigitSpan.IsEmpty &&
			int.TryParse(leftDigitSpan, out var leftNumber) &&
			int.TryParse(rightDigitSpan, out var rightNumber))
		{
			return leftNumber.CompareTo(rightNumber);
		}

		return leftSpan.CompareTo(rightSpan, m_comparison);
	}

	private int GetCommonNonDigitSuffix(ReadOnlySpan<char> left, ReadOnlySpan<char> right)
	{
		int commonLength;
		for (commonLength = 0; commonLength < left.Length && commonLength < right.Length; commonLength++)
		{


			if (GetComparableCharacter(left[left.Length - commonLength - 1], m_comparison) !=
				GetComparableCharacter(right[right.Length - commonLength - 1], m_comparison))
			{
				break;
			}
		}
		return commonLength;
	}

	private static char GetComparableCharacter(char c, StringComparison comparison)
	{
		return comparison switch
		{
			StringComparison.CurrentCultureIgnoreCase => char.ToLower(c, CultureInfo.CurrentCulture),
			_ => throw new NotImplementedException(),
		};
	}

	private static ReadOnlySpan<char> GetDigitSpan(ReadOnlySpan<char> span, int offset)
	{
		var start = -1;
		var length = 0;

		if (offset >= span.Length || !char.IsDigit(span[offset]))
			return [];

		int i = offset;
		while (i >= 0 && char.IsDigit(span[i]))
		{
			start = i;
			length++;
			i--;
		}
		i = offset + 1;
		while (i < span.Length && char.IsDigit(span[i]))
		{
			length++;
			i++;
		}

		return start == -1 || start + length != span.Length ? [] : span.Slice(start, length);
	}

	private StringComparison m_comparison;
}
