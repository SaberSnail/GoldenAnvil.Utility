using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GoldenAnvil.Utility.Calendar;

public class StandardCalendar : ICalendar
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
		if (config.UseLeapYear)
		{
			m_daysInLeapYear = m_daysInStandardYear + 1;
			m_daysIn4Years = (4 * m_daysInStandardYear) + 1;
			m_daysIn100Years = (100 * m_daysInStandardYear) + 24;
			m_daysIn400Years = (400 * m_daysInStandardYear) + 97;
		}
		else
		{
			m_daysInLeapYear = m_daysInStandardYear;
			m_daysIn4Years = 4 * m_daysInStandardYear;
			m_daysIn100Years = 100 * m_daysInStandardYear;
			m_daysIn400Years = 400 * m_daysInStandardYear;
		}
	}

	public TimePoint CreateTimePoint(int year, int month, int day) =>
		CreateTimePoint(year, month, day, 0, 0, 0);

	public TimePoint CreateTimePoint(int year, int month, int day, int hour, int minute, int second)
	{
		if (month < 1 || month > m_daysInMonth.Count)
			throw new ArgumentException($"{nameof(month)} must be between 1 and {m_daysInMonth.Count}.");
		if (hour < 0 || hour > m_hoursInDay)
			throw new ArgumentException($"{nameof(hour)} must be between 0 and {m_hoursInDay}.");
		if (minute < 0 || minute > m_minutesInHour)
			throw new ArgumentException($"{nameof(minute)} must be between 0 and {m_minutesInHour}.");
		if (second < 0 || second > m_secondsInMinute)
			throw new ArgumentException($"{nameof(second)} must be between 0 and {m_secondsInMinute}.");

		var isLeapYear = IsLeapYear(year);
		if (day < 1 || day > m_daysInMonth[month - 1] + (isLeapYear ? 1 : 0))
			throw new ArgumentException($"{nameof(day)} must be between 1 and {m_daysInMonth[month - 1] + (isLeapYear ? 1 : 0)}.");

		var totalDays = GetDaysInYears(year);
		totalDays += m_daysToStartOfMonth[month - 1];
		if (IsLeapYear(year) && month > m_leapYearMonth)
			totalDays++;
		totalDays += day;

		var secondsForDays = totalDays * m_secondsInDay;
		var secondsForHours = hour * m_secondsInHour;
		var secondsForMinutes = minute * m_secondsInMinute;

		return new(secondsForDays + secondsForHours + secondsForMinutes + second, this);
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
		return new TimeOffset(secondsForDays + secondsForHours + secondsForMinutes + seconds, this);
	}

	public TimePoint GetStartOfDay(TimePoint time) =>
		new(m_secondsInDay * (time.TotalSeconds / m_secondsInDay), this);

	public string FormatTime(TimePoint point, TimeFormat format)
	{
		(var years, var months, var days, var hours, var minutes, var seconds) = SplitSeconds(point.TotalSeconds);
		var date = new DateTime((int) years, (int) months, (int) days, (int) hours, (int) minutes, (int) seconds);
		return date.ToString(m_getFormatStringFunc(format), CultureInfo.CurrentCulture);
	}

	public string FormatOffsetFrom(TimePoint point, TimeOffset offset, TimeFormat format)
	{
		if (offset.TotalSeconds < 0)
			throw new ArgumentException($"{nameof(offset)} must be positive");

		(var startYears, var startMonths, var startDays, var startHours, var startMinutes, var startSeconds) = SplitSeconds(point.TotalSeconds);
		(var endYears, var endMonths, var endDays, var endHours, var endMinutes, var endSeconds) = SplitSeconds((point + offset).TotalSeconds);

		var seconds = endSeconds >= startSeconds ? endSeconds - startSeconds : m_secondsInMinute - startSeconds + endSeconds;

		var minutes = endMinutes >= startMinutes ? endMinutes - startMinutes : m_minutesInHour - startMinutes + endMinutes;
		if (endSeconds < startSeconds)
			minutes--;

		var hours = endHours >= startHours ? endHours - startHours : m_hoursInDay - startHours + endHours;
		if (endMinutes < startMinutes)
			hours--;

		var days = endDays >= startDays ? endDays - startDays : GetDaysInMonth((int) startMonths, (int) startYears) - startDays + endDays;
		if (endHours < startHours)
			days--;

		var months = endMonths >= startMonths ? endMonths - startMonths : m_daysInMonth.Count - startMonths + endMonths;
		if (endDays < startDays)
			months--;

		var years = endYears - startYears;
		if (endMonths < startMonths)
			years--;

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

	private long GetDaysInYears(long years)
	{
		var remainingYears = years;
		var totalDays = (years / 400) * m_daysIn400Years;
		remainingYears = years % 400;
		totalDays += (remainingYears / 100) * m_daysIn100Years;
		remainingYears = remainingYears % 100;
		totalDays += (remainingYears / 4) * m_daysIn4Years;
		remainingYears = remainingYears % 4;

		if (remainingYears != 0 && IsLeapYear(years - remainingYears))
		{
			totalDays += m_daysInLeapYear;
			remainingYears--;
		}

		totalDays += remainingYears * m_daysInStandardYear;
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
		long years = 0;
		long months = 0;
		long days = 0;

		var remainingDays = totalDays;
		years = (remainingDays / m_daysIn400Years) * 400;
		remainingDays = remainingDays % m_daysIn400Years;
		years += (remainingDays / m_daysIn100Years) * 100;
		remainingDays = remainingDays % m_daysIn100Years;
		years += (remainingDays / m_daysIn4Years) * 4;
		remainingDays = remainingDays % m_daysIn4Years;
		var isLeapYear = IsLeapYear(years);
		if (remainingDays > (isLeapYear ? m_daysInLeapYear : m_daysInStandardYear))
		{
			years++;
			remainingDays -= isLeapYear ? m_daysInLeapYear : m_daysInStandardYear;

			years += remainingDays / m_daysInStandardYear;
			remainingDays = remainingDays % m_daysInStandardYear;
		}
		if (remainingDays == 0)
		{
			remainingDays = m_daysInStandardYear;
			years--;
		}

		isLeapYear = IsLeapYear(years);
		months = GetMonthFromDayOfYear((int) remainingDays, isLeapYear);
		remainingDays -= m_daysToStartOfMonth[(int) months - 1];
		if (isLeapYear && months > m_leapYearMonth)
			remainingDays--;
		days = (int) remainingDays;

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

	private long GetDaysInMonth(int month, int year)
	{
		var yearToCheck = year;
		var index = month - 1;
		if (month < 0)
		{
			index = 11;
			yearToCheck--;
		}

		var days = m_daysInMonth[index];
		if (index == m_leapYearMonth - 1 && IsLeapYear(yearToCheck))
			days++;

		return days;
	}

	private readonly IReadOnlyList<long> m_daysInMonth;
	private readonly IReadOnlyList<long> m_daysToStartOfMonth;
	private readonly long m_daysInStandardYear;
	private readonly long m_daysInLeapYear;
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
