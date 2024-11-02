using System;
using System.Diagnostics;

namespace GoldenAnvil.Utility.Calendar;

[DebuggerDisplay("{Seconds}")]
public readonly record struct TimePoint(long Seconds) : IComparable<TimePoint>
{
	public TimeOffset GetTimeTo(TimePoint that) => new(that.Seconds - Seconds);

	public int CompareTo(TimePoint that) => Seconds.CompareTo(that.Seconds);

	public static bool operator <(TimePoint left, TimePoint right) => left.CompareTo(right) < 0;

	public static bool operator <=(TimePoint left, TimePoint right) => left.CompareTo(right) <= 0;

	public static bool operator >(TimePoint left, TimePoint right) => left.CompareTo(right) > 0;

	public static bool operator >=(TimePoint left, TimePoint right) => left.CompareTo(right) >= 0;

	public static TimePoint operator -(TimePoint timePoint, TimeOffset timeOffset) =>
		new(timePoint.Seconds - timeOffset.SecondOffset);

	public static TimePoint operator +(TimePoint timePoint, TimeOffset timeOffset) =>
		new(timePoint.Seconds + timeOffset.SecondOffset);
}
