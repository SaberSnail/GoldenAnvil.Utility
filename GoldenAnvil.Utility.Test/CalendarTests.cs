using System.Globalization;
using GoldenAnvil.Utility.Calendar;
namespace GoldenAnvil.Utility.Test;

[TestFixture]
public class CalendarTests
{
	[OneTimeSetUp]
	public void Setup()
	{
		CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CreateSpecificCulture("en-US");
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
	}

	[TestCase(2000, 1, 1)]
	[TestCase(2001, 1, 1)]
	[TestCase(2004, 1, 1)]
	[TestCase(2100, 1, 1)]
	[TestCase(2400, 1, 1)]
	[TestCase(2001, 4, 1)]
	[TestCase(2001, 5, 1)]
	[TestCase(2001, 6, 1)]
	[TestCase(2001, 7, 1)]
	[TestCase(2001, 8, 1)]
	[TestCase(2001, 9, 1)]
	[TestCase(2001, 10, 1)]
	[TestCase(2001, 11, 1)]
	[TestCase(2001, 12, 1)]
	[TestCase(2001, 1, 31)]
	[TestCase(2001, 2, 1)]
	[TestCase(2001, 3, 31)]
	[TestCase(2001, 4, 30)]
	[TestCase(2001, 5, 31)]
	[TestCase(2001, 6, 30)]
	[TestCase(2001, 7, 31)]
	[TestCase(2001, 8, 31)]
	[TestCase(2001, 9, 30)]
	[TestCase(2001, 10, 31)]
	[TestCase(2001, 11, 30)]
	[TestCase(2000, 2, 29)]
	[TestCase(2001, 2, 28)]
	[TestCase(2004, 2, 29)]
	[TestCase(2100, 2, 28)]
	[TestCase(2400, 2, 29)]
	[TestCase(2000, 3, 1)]
	[TestCase(2001, 3, 1)]
	[TestCase(2004, 3, 1)]
	[TestCase(2100, 3, 1)]
	[TestCase(2400, 3, 1)]
	[TestCase(2000, 12, 31)]
	[TestCase(2001, 12, 31)]
	[TestCase(2004, 12, 31)]
	[TestCase(2100, 12, 31)]
	[TestCase(2101, 1, 1)]
	[TestCase(2400, 12, 31)]
	[TestCase(1, 1, 1)]
	[TestCase(9999, 12, 31)]
	public void TestPointParts(int year, int month, int day)
	{
		var calendar = StandardCalendar.CreateStandard();
		var point = calendar.CreateTimePoint(year, month, day, 13, 14, 15);
		Assert.That(point.Parts, Is.EqualTo(new TimePointParts(year, month, day, 13, 14, 15)));
	}

	[TestCase(2000, 1, 1, -1, 0, 1999, 12, 31, 0, 0, 0)]
	[TestCase(2000, 1, 1, 0, -1, 1999, 12, 31, 23, 59, 59)]
	[TestCase(1999, 12, 31, 1, 0, 2000, 1, 1, 0, 0, 0)]
	[TestCase(1999, 12, 31, 0, 1, 1999, 12, 31, 0, 0, 1)]
	[TestCase(2000, 2, 29, -1, 0, 2000, 2, 28, 0, 0, 0)]
	[TestCase(2000, 2, 29, 1, 0, 2000, 3, 1, 0, 0, 0)]
	[TestCase(2000, 3, 1, -1, 0, 2000, 2, 29, 0, 0, 0)]
	[TestCase(2000, 2, 28, 1, 0, 2000, 2, 29, 0, 0, 0)]
	[TestCase(2001, 2, 28, 1, 0, 2001, 3, 1, 0, 0, 0)]
	[TestCase(2001, 3, 1, -1, 0, 2001, 2, 28, 0, 0, 0)]
	[TestCase(2000, 1, 1, 365, 0, 2000, 12, 31, 0, 0, 0)]
	[TestCase(2000, 1, 1, 366, 0, 2001, 1, 1, 0, 0, 0)]
	[TestCase(2001, 1, 1, 364, 0, 2001, 12, 31, 0, 0, 0)]
	[TestCase(2001, 1, 1, 365, 0, 2002, 1, 1, 0, 0, 0)]
	[TestCase(2100, 1, 1, 364, 0, 2100, 12, 31, 0, 0, 0)]
	[TestCase(2100, 1, 1, 365, 0, 2101, 1, 1, 0, 0, 0)]
	[TestCase(2103, 12, 31, 1, 0, 2104, 1, 1, 0, 0, 0)]
	public void TestPointPartsWithOffset(int year, int month, int day, int dayOffset, int secondOffset,
		int expectedYear, int expectedMonth, int expectedDay, int expectedHour, int expectedMinute, int expectedSecond)
	{
		var calendar = StandardCalendar.CreateStandard();
		var point = calendar.CreateTimePoint(year, month, day);
		var offset = calendar.CreateTimeOffset(dayOffset, 0, 0, secondOffset);
		var adjustedPoint = point + offset;
		Assert.That(adjustedPoint.Parts,
			Is.EqualTo(new TimePointParts(expectedYear, expectedMonth, expectedDay, expectedHour, expectedMinute, expectedSecond)));
	}

	[Test]
	public void TestEveryDayMatchesDateTime()
	{
		var calendar = StandardCalendar.CreateStandard();
		var oneDay = calendar.CreateTimeOffset(1, 0, 0, 0);
		var point = calendar.CreateTimePoint(1600, 1, 1);
		for (var date = new DateTime(1600, 1, 1); date.Year <= 2400; date = date.AddDays(1))
		{
			if (calendar.CreateTimePoint(date.Year, date.Month, date.Day) != point)
				Assert.Fail($"Creating {date:d} gave a different time than adding days up to it.");

			var parts = point.Parts;
			if (parts.Year != date.Year || parts.Month != date.Month || parts.Day != date.Day)
				Assert.Fail($"Adding days up to {date:d} gave {parts}.");

			point += oneDay;
		}
	}

	[Test]
	public void TestCalendarWithoutLeapYears()
	{
		var calendar = new StandardCalendar(new StandardCalendarConfig
		{
			DaysInMonth = [7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7],
			HoursInDay = 24,
			MinutesInHour = 60,
			SecondsInMinute = 60,
			GetFormatStringFunc = _ => "G",
		});
		var oneDay = calendar.CreateTimeOffset(1, 0, 0, 0);
		Assert.Multiple(() =>
		{
			Assert.That((calendar.CreateTimePoint(4, 12, 7) + oneDay).Parts, Is.EqualTo(new TimePointParts(5, 1, 1, 0, 0, 0)));
			Assert.That((calendar.CreateTimePoint(4, 1, 7) + oneDay).Parts, Is.EqualTo(new TimePointParts(4, 2, 1, 0, 0, 0)));
			Assert.That((calendar.CreateTimePoint(4, 1, 1) + calendar.CreateTimeOffset(84, 0, 0, 0)).Parts, Is.EqualTo(new TimePointParts(5, 1, 1, 0, 0, 0)));
			Assert.Throws<ArgumentException>(() => calendar.CreateTimePoint(4, 2, 8));
		});
	}

	[TestCase(TimeFormat.Short, "1/2/2001 3:04:05 AM")]
	[TestCase(TimeFormat.Long, "Tuesday, January 2, 2001 3:04:05 AM")]
	public void TestPointFormat(TimeFormat format, string expectedOutput)
	{
		var calendar = StandardCalendar.CreateStandard();
		Assert.That(calendar.FormatTime(calendar.CreateTimePoint(2001, 1, 2, 3, 4, 5), format), Is.EqualTo(expectedOutput));
	}

	[TestCase(1, 0, 0, 0, 1, 0, 0, 0)]
	[TestCase(0, 25, 61, 61, 1, 2, 2, 1)]
	[TestCase(0, 0, 0, 0, 0, 0, 0, 0)]
	[TestCase(-1, -1, 0, 0, -1, -1, 0, 0)]
	[TestCase(0, 0, -1, 30, 0, 0, 0, -30)]
	public void TestOffsetParts(int days, int hours, int minutes, int seconds,
		long expectedDays, int expectedHours, int expectedMinutes, int expectedSeconds)
	{
		var calendar = StandardCalendar.CreateStandard();
		var offset = calendar.CreateTimeOffset(days, hours, minutes, seconds);
		Assert.That(offset.Parts, Is.EqualTo(new TimeOffsetParts(expectedDays, expectedHours, expectedMinutes, expectedSeconds)));
	}

	[TestCase(2000, 1, 1, 1, TimeFormat.Long, "1 day")]
	[TestCase(2000, 1, 31, 1, TimeFormat.Long, "1 day")]
	[TestCase(2000, 2, 28, 1, TimeFormat.Long, "1 day")]
	[TestCase(2000, 2, 29, 1, TimeFormat.Long, "1 day")]
	[TestCase(2000, 12, 31, 1, TimeFormat.Long, "1 day")]
	[TestCase(2001, 1, 1, 1, TimeFormat.Long, "1 day")]
	[TestCase(2001, 1, 31, 1, TimeFormat.Long, "1 day")]
	[TestCase(2001, 2, 27, 1, TimeFormat.Long, "1 day")]
	[TestCase(2001, 2, 28, 1, TimeFormat.Long, "1 day")]
	[TestCase(2001, 12, 31, 1, TimeFormat.Long, "1 day")]
	[TestCase(2000, 1, 1, 30, TimeFormat.Long, "30 days")]
	[TestCase(2000, 1, 1, 31, TimeFormat.Long, "1 month")]
	[TestCase(2000, 1, 1, 32, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2000, 1, 31, 32, TimeFormat.Long, "1 month, 3 days")]
	[TestCase(2000, 1, 31, 29, TimeFormat.Long, "29 days")]
	[TestCase(2000, 2, 28, 32, TimeFormat.Long, "1 month, 3 days")]
	[TestCase(2000, 2, 28, 28, TimeFormat.Long, "28 days")]
	[TestCase(2000, 2, 28, 29, TimeFormat.Long, "1 month")]
	[TestCase(2000, 2, 29, 32, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2000, 2, 29, 27, TimeFormat.Long, "27 days")]
	[TestCase(2000, 12, 31, 32, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2000, 12, 31, 31, TimeFormat.Long, "1 month")]
	[TestCase(2001, 1, 1, 31, TimeFormat.Long, "1 month")]
	[TestCase(2001, 1, 1, 32, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2001, 1, 31, 32, TimeFormat.Long, "1 month, 4 days")]
	[TestCase(2001, 1, 31, 29, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2001, 2, 27, 32, TimeFormat.Long, "1 month, 4 days")]
	[TestCase(2001, 2, 27, 28, TimeFormat.Long, "1 month")]
	[TestCase(2001, 2, 28, 32, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2001, 2, 28, 27, TimeFormat.Long, "27 days")]
	[TestCase(2001, 12, 31, 32, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2001, 12, 31, 31, TimeFormat.Long, "1 month")]
	[TestCase(2001, 1, 1, 1, TimeFormat.Short, "1d")]
	public void TestOffsetFromFormat(int year, int month, int day, int dayOffset, TimeFormat format, string expectedOutput)
	{
		var calendar = StandardCalendar.CreateStandard();
		var point = calendar.CreateTimePoint(year, month, day);
		var offset = calendar.CreateTimeOffset(dayOffset, 0, 0, 0);
		var output = calendar.FormatOffsetFrom(point, offset, format);
		Assert.That(output, Is.EqualTo(expectedOutput));
	}

	[TestCase(2001, 1, 1, 10, 5, 30, 0, 0, 59, 40, "59 minutes, 40 seconds")]
	[TestCase(2001, 1, 1, 10, 0, 0, 30, 23, 0, 0, "30 days, 23 hours")]
	[TestCase(2000, 1, 15, 0, 0, 0, 361, 0, 0, 0, "11 months, 26 days")]
	public void TestOffsetFromFormatBorrowsFromZero(int year, int month, int day, int hour, int minute, int second,
		int dayOffset, int hourOffset, int minuteOffset, int secondOffset, string expectedOutput)
	{
		var calendar = StandardCalendar.CreateStandard();
		var point = calendar.CreateTimePoint(year, month, day, hour, minute, second);
		var offset = calendar.CreateTimeOffset(dayOffset, hourOffset, minuteOffset, secondOffset);
		Assert.That(calendar.FormatOffsetFrom(point, offset, TimeFormat.Long), Is.EqualTo(expectedOutput));
	}

	[TestCase(24, 0, 0)]
	[TestCase(0, 60, 0)]
	[TestCase(0, 0, 60)]
	[TestCase(-1, 0, 0)]
	public void TestTimeOutOfRangeThrows(int hour, int minute, int second)
	{
		var calendar = StandardCalendar.CreateStandard();
		Assert.Throws<ArgumentException>(() => calendar.CreateTimePoint(2001, 1, 1, hour, minute, second));
	}

	[TestCase(2004, 1, 32)]
	[TestCase(2004, 4, 31)]
	[TestCase(2004, 12, 32)]
	[TestCase(2001, 2, 29)]
	[TestCase(2100, 2, 29)]
	[TestCase(2001, 1, 0)]
	[TestCase(2001, 0, 1)]
	[TestCase(2001, 13, 1)]
	public void TestDayOutOfRangeThrows(int year, int month, int day)
	{
		var calendar = StandardCalendar.CreateStandard();
		Assert.Throws<ArgumentException>(() => calendar.CreateTimePoint(year, month, day));
	}

	[TestCase(0)]
	[TestCase(10000)]
	public void TestYearThatCannotBeFormattedThrows(int year)
	{
		var calendar = StandardCalendar.CreateStandard();
		Assert.Throws<ArgumentException>(() => calendar.CreateTimePoint(year, 1, 1));
	}

	[Test]
	public void TestSubtractingLaterPointIsNegative()
	{
		var calendar = StandardCalendar.CreateStandard();
		var earlier = calendar.CreateTimePoint(2001, 1, 1);
		var later = calendar.CreateTimePoint(2001, 1, 2);
		Assert.Multiple(() =>
		{
			Assert.That(later + (earlier - later), Is.EqualTo(earlier));
			Assert.That((earlier - later).Parts, Is.EqualTo(new TimeOffsetParts(-1, 0, 0, 0)));
		});
	}

	[Test]
	public void TestSubtractingPointsFromDifferentCalendarsThrows()
	{
		var point1 = StandardCalendar.CreateStandard().CreateTimePoint(2001, 1, 1);
		var point2 = StandardCalendar.CreateStandard().CreateTimePoint(2001, 1, 1);
		Assert.Throws<InvalidOperationException>(() => _ = point1 - point2);
	}

	[TestCase(1, 1, 1, 0, 0, 0)]
	[TestCase(2000, 2, 29, 13, 14, 15)]
	[TestCase(9999, 12, 31, 23, 59, 59)]
	public void TestSaveAndLoad(int year, int month, int day, int hour, int minute, int second)
	{
		var calendar = StandardCalendar.CreateStandard();
		var point = calendar.CreateTimePoint(year, month, day, hour, minute, second);
		Assert.That(calendar.LoadFromString(point.SaveToString()), Is.EqualTo(point));
	}

	[Test]
	public void TestSaveIsTheSameInEveryCulture()
	{
		var calendar = StandardCalendar.CreateStandard();
		var point = calendar.CreateTimePoint(2001, 1, 2, 3, 4, 5);
		var originalCulture = CultureInfo.CurrentCulture;
		try
		{
			var saved = point.SaveToString();
			CultureInfo.CurrentCulture = CultureInfo.CreateSpecificCulture("de-DE");
			Assert.Multiple(() =>
			{
				Assert.That(point.SaveToString(), Is.EqualTo(saved));
				Assert.That(calendar.LoadFromString(saved), Is.EqualTo(point));
			});
		}
		finally
		{
			CultureInfo.CurrentCulture = originalCulture;
		}
	}

	[TestCase("")]
	[TestCase(null)]
	[TestCase("abc")]
	[TestCase("1.5")]
	[TestCase("1,000")]
	public void TestLoadInvalidTextThrows(string? text)
	{
		var calendar = StandardCalendar.CreateStandard();
		Assert.Throws<FormatException>(() => calendar.LoadFromString(text!));
	}

	[Test]
	public void TestLoadOutOfRangeThrows()
	{
		var calendar = StandardCalendar.CreateStandard();
		var first = calendar.CreateTimePoint(1, 1, 1);
		var last = calendar.CreateTimePoint(9999, 12, 31, 23, 59, 59);
		var oneSecond = calendar.CreateTimeOffset(0, 0, 0, 1);
		Assert.Multiple(() =>
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => calendar.LoadFromString((first - oneSecond).SaveToString()));
			Assert.Throws<ArgumentOutOfRangeException>(() => calendar.LoadFromString((last + oneSecond).SaveToString()));
		});
	}

	[Test]
	public void TestTimePointEquality()
	{
		var calendar = StandardCalendar.CreateStandard();
		var point1 = calendar.CreateTimePoint(2001, 1, 1);
		var point2 = calendar.CreateTimePoint(2001, 1, 1);
		var point3 = calendar.CreateTimePoint(2001, 1, 2);
		var otherCalendarPoint = StandardCalendar.CreateStandard().CreateTimePoint(2001, 1, 1);
		Assert.Multiple(() =>
		{
			Assert.That(point1.Equals(point2), Is.True);
			Assert.That(point1 == point2, Is.True);
			Assert.That(point1 != point2, Is.False);
			Assert.That(point1.GetHashCode(), Is.EqualTo(point2.GetHashCode()));
			Assert.That(point1.Equals(point3), Is.False);
			Assert.That(point1 == point3, Is.False);
			Assert.That(point1 == otherCalendarPoint, Is.False);
			Assert.That(point1.CompareTo(null), Is.GreaterThan(0));
			Assert.That(point1 == null, Is.False);
		});
	}

	[Test]
	public void TestTimeOffsetEquality()
	{
		var calendar = StandardCalendar.CreateStandard();
		var offset1 = calendar.CreateTimeOffset(1, 0, 0, 0);
		var offset2 = calendar.CreateTimeOffset(0, 24, 0, 0);
		var offset3 = calendar.CreateTimeOffset(0, 23, 0, 0);
		Assert.Multiple(() =>
		{
			Assert.That(offset1.Equals(offset2), Is.True);
			Assert.That(offset1 == offset2, Is.True);
			Assert.That(offset1 != offset2, Is.False);
			Assert.That(offset1.GetHashCode(), Is.EqualTo(offset2.GetHashCode()));
			Assert.That(offset1.Equals(offset3), Is.False);
			Assert.That(offset1 == offset3, Is.False);
			Assert.That(offset1.CompareTo(null), Is.GreaterThan(0));
			Assert.That(offset1 == null, Is.False);
		});
	}
}
