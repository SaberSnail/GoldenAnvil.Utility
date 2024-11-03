using System;
using System.Diagnostics;

namespace GoldenAnvil.Utility.Calendar;

[DebuggerDisplay("{SecondOffset}")]
public class TimeOffset : IComparable<TimeOffset>
{
	internal TimeOffset(long offsetSeconds, ICalendar calendar)
	{
		TotalSeconds = offsetSeconds;
		Calendar = calendar;
	}

	public long TotalSeconds { get; }

	internal ICalendar Calendar { get; }

	public string RenderTimeFrom(TimePoint point, TimeFormat format)
	{
		if (Calendar != point.Calendar)
			throw new InvalidOperationException("Time point and offset must be created with the same calendar.");
		return Calendar.FormatOffsetFrom(point, this, format);
	}

	public int CompareTo(TimeOffset that)
	{
		if (Calendar != that.Calendar)
			throw new InvalidOperationException("Offsets must be created with the same calendar.");
		return TotalSeconds.CompareTo(that.TotalSeconds);
	}

	public static bool operator <(TimeOffset left, TimeOffset right) => left.CompareTo(right) < 0;

	public static bool operator <=(TimeOffset left, TimeOffset right) => left.CompareTo(right) <= 0;

	public static bool operator >(TimeOffset left, TimeOffset right) => left.CompareTo(right) > 0;

	public static bool operator >=(TimeOffset left, TimeOffset right) => left.CompareTo(right) >= 0;

	public static TimeOffset operator -(TimeOffset left, TimeOffset right)
	{
		if (left.Calendar != right.Calendar)
			throw new InvalidOperationException("Offsets must be created with the same calendar.");
		return new(left.TotalSeconds - right.TotalSeconds, left.Calendar);
	}

	public static TimeOffset operator +(TimeOffset left, TimeOffset right)
	{
		if (left.Calendar != right.Calendar)
			throw new InvalidOperationException("Offsets must be created with the same calendar.");
		return new(left.TotalSeconds + right.TotalSeconds, left.Calendar);
	}
}
