namespace GoldenAnvil.Utility.Calendar;

public sealed class StandardCalendar : StandardCalendarBase
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
		: base(config)
	{
	}
}
