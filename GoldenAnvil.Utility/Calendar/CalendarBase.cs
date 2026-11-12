using System;

namespace GoldenAnvil.Utility.Calendar;

/// <summary>
/// The base class for calendars. A calendar creates <see cref="TimePoint"/> and <see cref="TimeOffset"/> objects in its own
/// terms (such as years, months, and days) and formats them. How a time is stored is known only to calendars, through the
/// protected members here; code that uses a calendar works only with time points and offsets.
/// </summary>
public abstract class CalendarBase
{
	/// <summary>
	/// Reads a time written by <see cref="TimePoint.SaveToString"/>.
	/// </summary>
	/// <exception cref="FormatException">The text isn't a saved time.</exception>
	/// <exception cref="ArgumentOutOfRangeException">The time is outside the range this calendar supports.</exception>
	public TimePoint LoadFromString(string text)
	{
		var point = CreateTimePointFromTotalSeconds(TimePoint.ReadSavedTotalSeconds(text));
		if (!IsSupported(point))
			throw new ArgumentOutOfRangeException(nameof(text), text, "The saved time is outside the range this calendar supports.");
		return point;
	}

	public abstract string FormatTime(TimePoint point, TimeFormat format);

	public abstract string FormatOffsetFrom(TimePoint point, TimeOffset offset, TimeFormat format);

	/// <summary>
	/// Whether this calendar can work with the time, such as being within the years it supports.
	/// </summary>
	protected virtual bool IsSupported(TimePoint point) => true;

	protected TimePoint CreateTimePointFromTotalSeconds(long totalSeconds) => new(totalSeconds, this);

	protected TimeOffset CreateTimeOffsetFromTotalSeconds(long totalSeconds) => new(totalSeconds, this);

	/// <exception cref="InvalidOperationException">The time point was created by a different calendar.</exception>
	protected long GetTotalSeconds(TimePoint point)
	{
		if (point.Calendar != this)
			throw new InvalidOperationException("The time point was created by a different calendar.");
		return point.TotalSeconds;
	}

	/// <exception cref="InvalidOperationException">The time offset was created by a different calendar.</exception>
	protected long GetTotalSeconds(TimeOffset offset)
	{
		if (offset.Calendar != this)
			throw new InvalidOperationException("The time offset was created by a different calendar.");
		return offset.TotalSeconds;
	}
}
