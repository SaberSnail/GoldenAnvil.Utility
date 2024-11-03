using System;
using System.Diagnostics;

namespace GoldenAnvil.Utility.Calendar;

[DebuggerDisplay("{Seconds}")]
public class TimePoint : IComparable<TimePoint>
{
	internal TimePoint(long seconds, ICalendar calendar)
	{
		TotalSeconds = seconds;
		Calendar = calendar;
	}

	public long TotalSeconds { get; }

	internal ICalendar Calendar { get; }

	public string Render(TimeFormat format) => Calendar.FormatTime(this, format);

	public string RenderTimeTo(TimeOffset offset, TimeFormat format)
	{
		if (Calendar != offset.Calendar)
			throw new InvalidOperationException("Time point and offset must be created with the same calendar.");
		return Calendar.FormatOffsetFrom(this, offset, format);
	}

	public int CompareTo(TimePoint that)
	{
		if (Calendar != that.Calendar)
			throw new InvalidOperationException("Points must be created with the same calendar.");
		return TotalSeconds.CompareTo(that.TotalSeconds);
	}

	public static bool operator <(TimePoint left, TimePoint right) => left.CompareTo(right) < 0;

	public static bool operator <=(TimePoint left, TimePoint right) => left.CompareTo(right) <= 0;

	public static bool operator >(TimePoint left, TimePoint right) => left.CompareTo(right) > 0;

	public static bool operator >=(TimePoint left, TimePoint right) => left.CompareTo(right) >= 0;

	public static TimeOffset operator -(TimePoint point1, TimePoint point2)
	{
		if (point1.Calendar != point1.Calendar)
			throw new InvalidOperationException("Points must be created with the same calendar.");
		return new(Math.Abs(point1.TotalSeconds - point2.TotalSeconds), point1.Calendar);
	}

	public static TimePoint operator -(TimePoint point, TimeOffset offset)
	{
		if (point.Calendar != offset.Calendar)
			throw new InvalidOperationException("Time point and offset must be created with the same calendar.");
		return new(point.TotalSeconds - offset.TotalSeconds, point.Calendar);
	}

	public static TimePoint operator +(TimePoint point, TimeOffset offset)
	{
		if (point.Calendar != offset.Calendar)
			throw new InvalidOperationException("Time point and offset use different calendars.");
		return new(point.TotalSeconds + offset.TotalSeconds, point.Calendar);
	}
}
