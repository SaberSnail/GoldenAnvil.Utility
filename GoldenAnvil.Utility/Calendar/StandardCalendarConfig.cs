using System;
using System.Collections.Generic;

namespace GoldenAnvil.Utility.Calendar;

public sealed class StandardCalendarConfig
{
	public required IReadOnlyList<int> DaysInMonth { get; init; }
	public required int HoursInDay { get; init; }
	public required int MinutesInHour { get; init; }
	public required int SecondsInMinute { get; init; }

	/// <summary>
	/// If true, will use standard leap year rules to add an extra day to the leap year month
	/// </summary>
	public bool UseLeapYear { get; init; }
	public int? LeapYearMonth { get; init; }

	public required Func<TimeFormat, string> GetFormatStringFunc { get; init; }
}
