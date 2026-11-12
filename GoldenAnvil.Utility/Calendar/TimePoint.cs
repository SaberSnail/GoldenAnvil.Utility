using System;
using System.Diagnostics;
using System.Globalization;

namespace GoldenAnvil.Utility.Calendar;

[DebuggerDisplay("{TotalSeconds}")]
public class TimePoint : IComparable<TimePoint>, IEquatable<TimePoint>
{
	internal TimePoint(long seconds, CalendarBase calendar)
	{
		TotalSeconds = seconds;
		Calendar = calendar;
	}

	internal long TotalSeconds { get; }

	/// <summary>
	/// Writes this time as text that <see cref="CalendarBase.LoadFromString"/> reads back, the same in every culture.
	/// </summary>
	public string SaveToString() => TotalSeconds.ToString(CultureInfo.InvariantCulture);

	internal static long ReadSavedTotalSeconds(string text)
	{
		if (!long.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var totalSeconds))
			throw new FormatException($"\"{text}\" isn't a saved time.");
		return totalSeconds;
	}

	internal CalendarBase Calendar { get; }

	public string Render(TimeFormat format) => Calendar.FormatTime(this, format);

	public string RenderTimeTo(TimeOffset offset, TimeFormat format)
	{
		if (Calendar != offset.Calendar)
			throw new InvalidOperationException("Time point and offset must be created with the same calendar.");
		return Calendar.FormatOffsetFrom(this, offset, format);
	}

	public int CompareTo(TimePoint that)
	{
		if (that is null)
			return 1;
		if (Calendar != that.Calendar)
			throw new InvalidOperationException("Points must be created with the same calendar.");
		return TotalSeconds.CompareTo(that.TotalSeconds);
	}

	public bool Equals(TimePoint that) =>
		that is not null && Calendar == that.Calendar && TotalSeconds == that.TotalSeconds;

	public override bool Equals(object obj) => Equals(obj as TimePoint);

	public override int GetHashCode() => TotalSeconds.GetHashCode();

	public static bool operator ==(TimePoint left, TimePoint right) => left?.Equals(right) ?? right is null;

	public static bool operator !=(TimePoint left, TimePoint right) => !(left == right);

	public static bool operator <(TimePoint left, TimePoint right) => left.CompareTo(right) < 0;

	public static bool operator <=(TimePoint left, TimePoint right) => left.CompareTo(right) <= 0;

	public static bool operator >(TimePoint left, TimePoint right) => left.CompareTo(right) > 0;

	public static bool operator >=(TimePoint left, TimePoint right) => left.CompareTo(right) >= 0;

	public static TimeOffset operator -(TimePoint point1, TimePoint point2)
	{
		if (point1.Calendar != point2.Calendar)
			throw new InvalidOperationException("Points must be created with the same calendar.");
		return new(point1.TotalSeconds - point2.TotalSeconds, point1.Calendar);
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
