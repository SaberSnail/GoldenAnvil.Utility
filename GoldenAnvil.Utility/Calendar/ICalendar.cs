namespace GoldenAnvil.Utility.Calendar;

public interface ICalendar
{
	TimePoint CreateTimePoint(int year, int month, int day);
	TimePoint CreateTimePoint(int year, int month, int day, int hours, int minutes, int seconds);
	TimePoint CreateTimePoint(TimePoint date, int hours, int minutes, int seconds);
	TimeOffset CreateTimeOffset(int days, int hours, int minutes, int seconds);
	TimePoint GetStartOfDay(TimePoint time);
	string FormatTime(TimePoint point, TimeFormat format);
	string FormatOffsetFrom(TimePoint point, TimeOffset offset, TimeFormat format);
}
