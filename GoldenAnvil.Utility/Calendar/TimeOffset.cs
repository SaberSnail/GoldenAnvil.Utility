using System;
using System.Diagnostics;

namespace GoldenAnvil.Utility.Calendar;

[DebuggerDisplay("{SecondOffset}")]
public readonly record struct TimeOffset(long SecondOffset) : IComparable<TimeOffset>
{
	public static readonly TimeOffset OneSecond = new(1);

	public int CompareTo(TimeOffset that) => SecondOffset.CompareTo(that.SecondOffset);

	public static bool operator <(TimeOffset left, TimeOffset right) => left.CompareTo(right) < 0;

	public static bool operator <=(TimeOffset left, TimeOffset right) => left.CompareTo(right) <= 0;

	public static bool operator >(TimeOffset left, TimeOffset right) => left.CompareTo(right) > 0;

	public static bool operator >=(TimeOffset left, TimeOffset right) => left.CompareTo(right) >= 0;

	public static TimeOffset operator -(TimeOffset left, TimeOffset right) =>
		new(left.SecondOffset - right.SecondOffset);

	public static TimeOffset operator +(TimeOffset left, TimeOffset right) =>
		new(left.SecondOffset + right.SecondOffset);
}
