using System;
using System.Diagnostics;

namespace GoldenAnvil.Utility.Calendar;

[DebuggerDisplay("{TotalSeconds}")]
public class TimeOffset : IComparable<TimeOffset>, IEquatable<TimeOffset>
{
	internal TimeOffset(long offsetSeconds, CalendarBase calendar)
	{
		TotalSeconds = offsetSeconds;
		Calendar = calendar;
	}

	internal long TotalSeconds { get; }

	internal CalendarBase Calendar { get; }

	public string RenderTimeFrom(TimePoint point, TimeFormat format)
	{
		if (Calendar != point.Calendar)
			throw new InvalidOperationException("Time point and offset must be created with the same calendar.");
		return Calendar.FormatOffsetFrom(point, this, format);
	}

	public int CompareTo(TimeOffset that)
	{
		if (that is null)
			return 1;
		if (Calendar != that.Calendar)
			throw new InvalidOperationException("Offsets must be created with the same calendar.");
		return TotalSeconds.CompareTo(that.TotalSeconds);
	}

	public bool Equals(TimeOffset that) =>
		that is not null && Calendar == that.Calendar && TotalSeconds == that.TotalSeconds;

	public override bool Equals(object obj) => Equals(obj as TimeOffset);

	public override int GetHashCode() => TotalSeconds.GetHashCode();

	public static bool operator ==(TimeOffset left, TimeOffset right) => left?.Equals(right) ?? right is null;

	public static bool operator !=(TimeOffset left, TimeOffset right) => !(left == right);

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
