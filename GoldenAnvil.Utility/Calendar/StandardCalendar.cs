using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GoldenAnvil.Utility.Calendar;

public class StandardCalendar : CalendarBase
{
	public static StandardCalendar CreateStandard() => new(
		new StandardCalendarConfig
		{
			DaysInMonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31],
			HoursInDay = 24,
			MinutesInHour = 60,
			SecondsInMinute = 60,
			UseLeapYear = true,
			LeapYearMonth = 2,
			GetFormatStringFunc = format => format == TimeFormat.Long ? "F" : "G",
		});

	public StandardCalendar(StandardCalendarConfig config)
	{
		m_daysInMonth = config.DaysInMonth.Select(x => (long) x).AsReadOnlyList();
		if (config.UseLeapYear && config.LeapYearMonth is null)
			throw new ArgumentException(nameof(config.LeapYearMonth));
		m_useLeapYear = config.UseLeapYear;
		m_leapYearMonth = config.LeapYearMonth;
		m_hoursInDay = config.HoursInDay;
		m_minutesInHour = config.MinutesInHour;
		m_secondsInMinute = config.SecondsInMinute;
		m_secondsInHour = config.MinutesInHour * config.SecondsInMinute;
		m_secondsInDay = config.HoursInDay * config.MinutesInHour * config.SecondsInMinute;
		m_getFormatStringFunc = config.GetFormatStringFunc;

		var total = 0L;
		m_daysToStartOfMonth = config.DaysInMonth
			.Select(x =>
			{
				var current = total;
				total += x;
				return current;
			}).AsReadOnlyList();
		m_daysInStandardYear = config.DaysInMonth.Sum();
		m_daysIn4Years = (4 * m_daysInStandardYear) + 1;
		m_daysIn100Years = (100 * m_daysInStandardYear) + 24;
		m_daysIn400Years = (400 * m_daysInStandardYear) + 97;
	}

	public TimePoint CreateTimePoint(int year, int month, int day) =>
		CreateTimePoint(year, month, day, 0, 0, 0);

	public TimePoint CreateTimePoint(int year, int month, int day, int hour, int minute, int second)
	{
		if (year < c_minYear || year > c_maxYear)
			throw new ArgumentException($"{nameof(year)} must be between {c_minYear} and {c_maxYear}.");
		if (month < 1 || month > m_daysInMonth.Count)
			throw new ArgumentException($"{nameof(month)} must be between 1 and {m_daysInMonth.Count}.");
		if (hour < 0 || hour >= m_hoursInDay)
			throw new ArgumentException($"{nameof(hour)} must be between 0 and {m_hoursInDay - 1}.");
		if (minute < 0 || minute >= m_minutesInHour)
			throw new ArgumentException($"{nameof(minute)} must be between 0 and {m_minutesInHour - 1}.");
		if (second < 0 || second >= m_secondsInMinute)
			throw new ArgumentException($"{nameof(second)} must be between 0 and {m_secondsInMinute - 1}.");

		var daysInMonth = GetDaysInMonth(month, year);
		if (day < 1 || day > daysInMonth)
			throw new ArgumentException($"{nameof(day)} must be between 1 and {daysInMonth}.");

		var totalDays = GetDaysBeforeYear(year);
		totalDays += m_daysToStartOfMonth[month - 1];
		if (IsLeapYear(year) && month > m_leapYearMonth)
			totalDays++;
		totalDays += day - 1;

		var secondsForDays = totalDays * m_secondsInDay;
		var secondsForHours = hour * m_secondsInHour;
		var secondsForMinutes = minute * m_secondsInMinute;

		return CreateTimePointFromTotalSeconds(secondsForDays + secondsForHours + secondsForMinutes + second);
	}

	public TimePoint CreateTimePoint(TimePoint date, int hours, int minutes, int seconds)
	{
		var startOfDay = GetStartOfDay(date);
		return startOfDay + CreateTimeOffset(0, hours, minutes, seconds);
	}

	public TimeOffset CreateTimeOffset(int days, int hours, int minutes, int seconds)
	{
		var secondsForDays = days * m_secondsInDay;
		var secondsForHours = hours * m_secondsInHour;
		var secondsForMinutes = minutes * m_secondsInMinute;
		return CreateTimeOffsetFromTotalSeconds(secondsForDays + secondsForHours + secondsForMinutes + seconds);
	}

	public TimePoint GetStartOfDay(TimePoint time) =>
		CreateTimePointFromTotalSeconds(m_secondsInDay * (GetTotalSeconds(time) / m_secondsInDay));

	protected override bool IsSupported(TimePoint point)
	{
		var totalSeconds = GetTotalSeconds(point);
		return totalSeconds >= 0 && totalSeconds < GetDaysBeforeYear(c_maxYear + 1) * m_secondsInDay;
	}


	public StandardTimePointParts GetParts(TimePoint point)
	{
		(var years, var months, var days, var hours, var minutes, var seconds) = SplitSeconds(GetTotalSeconds(point));
		return new((int) years, (int) months, (int) days, (int) hours, (int) minutes, (int) seconds);
	}

	public StandardTimeOffsetParts GetParts(TimeOffset offset)
	{
		var totalSeconds = GetTotalSeconds(offset);
		return new(
			totalSeconds / m_secondsInDay,
			(int) (totalSeconds % m_secondsInDay / m_secondsInHour),
			(int) (totalSeconds % m_secondsInHour / m_secondsInMinute),
			(int) (totalSeconds % m_secondsInMinute));
	}

	public override string FormatTime(TimePoint point, TimeFormat format)
	{
		(var years, var months, var days, var hours, var minutes, var seconds) = SplitSeconds(GetTotalSeconds(point));
		var date = new DateTime((int) years, (int) months, (int) days, (int) hours, (int) minutes, (int) seconds);
		return date.ToString(m_getFormatStringFunc(format), CultureInfo.CurrentCulture);
	}

	public override string FormatOffsetFrom(TimePoint point, TimeOffset offset, TimeFormat format)
	{
		if (GetTotalSeconds(offset) < 0)
			throw new ArgumentException($"{nameof(offset)} must be positive");

		(var startYears, var startMonths, var startDays, var startHours, var startMinutes, var startSeconds) = SplitSeconds(GetTotalSeconds(point));
		(var endYears, var endMonths, var endDays, var endHours, var endMinutes, var endSeconds) = SplitSeconds(GetTotalSeconds(point + offset));

		// Subtract each part, borrowing from the next larger part when the result is negative.
		var borrow = 0L;
		var seconds = Subtract(endSeconds, startSeconds, m_secondsInMinute, ref borrow);
		var minutes = Subtract(endMinutes, startMinutes, m_minutesInHour, ref borrow);
		var hours = Subtract(endHours, startHours, m_hoursInDay, ref borrow);
		var days = Subtract(endDays, startDays, GetDaysInMonth(startMonths, startYears), ref borrow);
		var months = Subtract(endMonths, startMonths, m_daysInMonth.Count, ref borrow);
		var years = endYears - startYears - borrow;

		var terms = new List<string>();
		if (years != 0)
			terms.Add(OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetYearLong" : "TimeOffsetYearShort", years).FormatCurrentCulture(years));
		if (months != 0)
			terms.Add(OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetMonthLong" : "TimeOffsetMonthShort", months).FormatCurrentCulture(months));
		if (days != 0)
			terms.Add(OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetDayLong" : "TimeOffsetDayShort", days).FormatCurrentCulture(days));
		if (hours != 0)
			terms.Add(OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetHourLong" : "TimeOffsetHourShort", hours).FormatCurrentCulture(hours));
		if (minutes != 0)
			terms.Add(OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetMinuteLong" : "TimeOffsetMinuteShort", minutes).FormatCurrentCulture(minutes));
		if (seconds != 0)
			terms.Add(OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetSecondLong" : "TimeOffsetSecondShort", seconds).FormatCurrentCulture(seconds));

		if (terms.Count == 0)
			return OurResources.ResourceManager.Pluralize(format == TimeFormat.Long ? "TimeOffsetSecondLong" : "TimeOffsetSecondShort", 0).FormatCurrentCulture(0);

		string output = null;
		foreach (var term in terms)
		{
			if (output is null)
				output = term;
			else
				output = OurResources.TimeOffsetTermJoin.FormatCurrentCulture(output, term);
		}
		return output;
	}

	private static long Subtract(long end, long start, long unitsInNextPart, ref long borrow)
	{
		var difference = end - start - borrow;
		borrow = difference < 0 ? 1 : 0;
		return difference + (borrow * unitsInNextPart);
	}

	// Days are counted from the first day of year 1, as DateTime counts them.
	private long GetDaysBeforeYear(long year)
	{
		var previousYears = year - 1;
		var totalDays = previousYears * m_daysInStandardYear;
		if (m_useLeapYear)
			totalDays += (previousYears / 4) - (previousYears / 100) + (previousYears / 400);
		return totalDays;
	}

	private (long Years, long Months, long Days, long Hours, long Minutes, long Seconds) SplitSeconds(long totalSeconds)
	{
		var remainingSeconds = totalSeconds;
		var totalDays = remainingSeconds / m_secondsInDay;
		remainingSeconds = remainingSeconds % m_secondsInDay;

		(var years, var months, var days) = SplitDays(totalDays);

		long hours = remainingSeconds / m_secondsInHour;
		remainingSeconds = remainingSeconds % m_secondsInHour;
		long minutes = remainingSeconds / m_secondsInMinute;
		long seconds = remainingSeconds % m_secondsInMinute;

		return (years, months, days, hours, minutes, seconds);
	}

	private (long Years, long Months, long Days) SplitDays(long totalDays)
	{
		long years;
		long dayOfYear;
		if (m_useLeapYear)
		{
			// Each 400 year cycle starts at year 1 and ends with a leap year, so the last century in the cycle
			// and the last year in each 4 year block are one day longer; Math.Min keeps their last day in them.
			var remainingDays = totalDays;
			var cycles400 = remainingDays / m_daysIn400Years;
			remainingDays %= m_daysIn400Years;
			var cycles100 = Math.Min(remainingDays / m_daysIn100Years, 3);
			remainingDays -= cycles100 * m_daysIn100Years;
			var cycles4 = remainingDays / m_daysIn4Years;
			remainingDays %= m_daysIn4Years;
			var cycles1 = Math.Min(remainingDays / m_daysInStandardYear, 3);
			remainingDays -= cycles1 * m_daysInStandardYear;

			years = (cycles400 * 400) + (cycles100 * 100) + (cycles4 * 4) + cycles1 + 1;
			dayOfYear = remainingDays;
		}
		else
		{
			years = (totalDays / m_daysInStandardYear) + 1;
			dayOfYear = totalDays % m_daysInStandardYear;
		}

		var isLeapYear = IsLeapYear(years);
		long months = GetMonthFromDayOfYear((int) dayOfYear + 1, isLeapYear);
		var days = dayOfYear - m_daysToStartOfMonth[(int) months - 1] + 1;
		if (isLeapYear && months > m_leapYearMonth)
			days--;

		return (years, months, days);
	}

	private bool IsLeapYear(long year)
	{
		if (!m_useLeapYear)
			return false;
		if (year % 400 == 0)
			return true;
		if ((year % 4 == 0) && (year % 100 != 0))
			return true;
		return false;
	}

	private int GetMonthFromDayOfYear(int dayOfYear, bool isLeapYear)
	{
		var totalDays = 0L;
		for (int month = 1; month <= m_daysInMonth.Count; month++)
		{
			totalDays += m_daysInMonth[month - 1];
			if (isLeapYear && month == m_leapYearMonth)
				totalDays++;
			if (dayOfYear <= totalDays)
				return month;
		}
		throw new InvalidOperationException($"Invalid day of year is too large: {dayOfYear}");
	}

	private long GetDaysInMonth(long month, long year)
	{
		var days = m_daysInMonth[(int) month - 1];
		if (month == m_leapYearMonth && IsLeapYear(year))
			days++;
		return days;
	}

	// StandardCalendar formats times with DateTime, so it has the same range of years.
	private const int c_minYear = 1;
	private const int c_maxYear = 9999;

	private readonly IReadOnlyList<long> m_daysInMonth;
	private readonly IReadOnlyList<long> m_daysToStartOfMonth;
	private readonly long m_daysInStandardYear;
	private readonly long m_daysIn400Years;
	private readonly long m_daysIn100Years;
	private readonly long m_daysIn4Years;
	private readonly bool m_useLeapYear;
	private readonly int? m_leapYearMonth;
	private readonly long m_hoursInDay;
	private readonly long m_minutesInHour;
	private readonly long m_secondsInHour;
	private readonly long m_secondsInMinute;
	private readonly long m_secondsInDay;
	private readonly Func<TimeFormat, string> m_getFormatStringFunc;
}
