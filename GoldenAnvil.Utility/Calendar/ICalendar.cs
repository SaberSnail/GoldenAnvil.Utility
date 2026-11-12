namespace GoldenAnvil.Utility.Calendar;

public interface ICalendar
{
	TimePoint CreateTimePoint(int year, int month, int day);
	TimePoint CreateTimePoint(int year, int month, int day, int hours, int minutes, int seconds);
	TimePoint CreateTimePoint(TimePoint date, int hours, int minutes, int seconds);
	TimeOffset CreateTimeOffset(int days, int hours, int minutes, int seconds);
	TimePoint GetStartOfDay(TimePoint time);

	/// <summary>
	/// Reads a time written by <see cref="TimePoint.SaveToString"/>.
	/// </summary>
	/// <exception cref="System.FormatException">The text isn't a saved time.</exception>
	/// <exception cref="System.ArgumentOutOfRangeException">The time is outside the range this calendar supports.</exception>
	TimePoint LoadFromString(string text);

	TimePointParts GetParts(TimePoint point);
	TimeOffsetParts GetParts(TimeOffset offset);
	string FormatTime(TimePoint point, TimeFormat format);
	string FormatOffsetFrom(TimePoint point, TimeOffset offset, TimeFormat format);
}
