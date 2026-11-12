using System.Globalization;
using GoldenAnvil.Utility.Calendar;
namespace GoldenAnvil.Utility.Test;

[TestFixture]
public class CalendarTests
{
	[OneTimeSetUp]
	public void Setup()
	{
		CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CreateSpecificCulture("en-US");
	}

	[TestCase(2000, 1, 1, TimeFormat.Short, "1/1/2000 12:00:00 AM")]
	[TestCase(2001, 1, 1, TimeFormat.Short, "1/1/2001 12:00:00 AM")]
	[TestCase(2004, 1, 1, TimeFormat.Short, "1/1/2004 12:00:00 AM")]
	[TestCase(2100, 1, 1, TimeFormat.Short, "1/1/2100 12:00:00 AM")]
	[TestCase(2400, 1, 1, TimeFormat.Short, "1/1/2400 12:00:00 AM")]
	[TestCase(2001, 4, 1, TimeFormat.Short, "4/1/2001 12:00:00 AM")]
	[TestCase(2001, 5, 1, TimeFormat.Short, "5/1/2001 12:00:00 AM")]
	[TestCase(2001, 6, 1, TimeFormat.Short, "6/1/2001 12:00:00 AM")]
	[TestCase(2001, 7, 1, TimeFormat.Short, "7/1/2001 12:00:00 AM")]
	[TestCase(2001, 8, 1, TimeFormat.Short, "8/1/2001 12:00:00 AM")]
	[TestCase(2001, 9, 1, TimeFormat.Short, "9/1/2001 12:00:00 AM")]
	[TestCase(2001, 10, 1, TimeFormat.Short, "10/1/2001 12:00:00 AM")]
	[TestCase(2001, 11, 1, TimeFormat.Short, "11/1/2001 12:00:00 AM")]
	[TestCase(2001, 12, 1, TimeFormat.Short, "12/1/2001 12:00:00 AM")]
	[TestCase(2001, 1, 31, TimeFormat.Short, "1/31/2001 12:00:00 AM")]
	[TestCase(2001, 2, 1, TimeFormat.Short, "2/1/2001 12:00:00 AM")]
	[TestCase(2001, 3, 31, TimeFormat.Short, "3/31/2001 12:00:00 AM")]
	[TestCase(2001, 4, 30, TimeFormat.Short, "4/30/2001 12:00:00 AM")]
	[TestCase(2001, 5, 31, TimeFormat.Short, "5/31/2001 12:00:00 AM")]
	[TestCase(2001, 6, 30, TimeFormat.Short, "6/30/2001 12:00:00 AM")]
	[TestCase(2001, 7, 31, TimeFormat.Short, "7/31/2001 12:00:00 AM")]
	[TestCase(2001, 8, 31, TimeFormat.Short, "8/31/2001 12:00:00 AM")]
	[TestCase(2001, 9, 30, TimeFormat.Short, "9/30/2001 12:00:00 AM")]
	[TestCase(2001, 10, 31, TimeFormat.Short, "10/31/2001 12:00:00 AM")]
	[TestCase(2001, 11, 30, TimeFormat.Short, "11/30/2001 12:00:00 AM")]
	[TestCase(2000, 2, 29, TimeFormat.Short, "2/29/2000 12:00:00 AM")]
	[TestCase(2001, 2, 28, TimeFormat.Short, "2/28/2001 12:00:00 AM")]
	[TestCase(2004, 2, 29, TimeFormat.Short, "2/29/2004 12:00:00 AM")]
	[TestCase(2100, 2, 28, TimeFormat.Short, "2/28/2100 12:00:00 AM")]
	[TestCase(2400, 2, 29, TimeFormat.Short, "2/29/2400 12:00:00 AM")]
	[TestCase(2000, 3, 1, TimeFormat.Short, "3/1/2000 12:00:00 AM")]
	[TestCase(2001, 3, 1, TimeFormat.Short, "3/1/2001 12:00:00 AM")]
	[TestCase(2004, 3, 1, TimeFormat.Short, "3/1/2004 12:00:00 AM")]
	[TestCase(2100, 3, 1, TimeFormat.Short, "3/1/2100 12:00:00 AM")]
	[TestCase(2400, 3, 1, TimeFormat.Short, "3/1/2400 12:00:00 AM")]
	[TestCase(2000, 12, 31, TimeFormat.Short, "12/31/2000 12:00:00 AM")]
	[TestCase(2001, 12, 31, TimeFormat.Short, "12/31/2001 12:00:00 AM")]
	[TestCase(2004, 12, 31, TimeFormat.Short, "12/31/2004 12:00:00 AM")]
	[TestCase(2100, 1, 1, TimeFormat.Short, "1/1/2100 12:00:00 AM")]
	[TestCase(2100, 12, 31, TimeFormat.Short, "12/31/2100 12:00:00 AM")]
	[TestCase(2101, 1, 1, TimeFormat.Short, "1/1/2101 12:00:00 AM")]
	[TestCase(2400, 12, 31, TimeFormat.Short, "12/31/2400 12:00:00 AM")]
	public void TestPointFormat(int year, int month, int day, TimeFormat format, string expectedOutput)
	{
		var calendar = StandardCalendar.CreateStandard();
		Assert.That(calendar.FormatTime(calendar.CreateTimePoint(year, month, day), format), Is.EqualTo(expectedOutput));
	}

	[TestCase(2000, 1, 1, -1, 0, TimeFormat.Short, "12/31/1999 12:00:00 AM")]
	[TestCase(2000, 1, 1, 0, -1, TimeFormat.Short, "12/31/1999 11:59:59 PM")]
	[TestCase(1999, 12, 31, 1, 0, TimeFormat.Short, "1/1/2000 12:00:00 AM")]
	[TestCase(1999, 12, 31, 0, 1, TimeFormat.Short, "12/31/1999 12:00:01 AM")]
	[TestCase(2000, 2, 29, -1, 0, TimeFormat.Short, "2/28/2000 12:00:00 AM")]
	[TestCase(2000, 2, 29, 1, 0, TimeFormat.Short, "3/1/2000 12:00:00 AM")]
	[TestCase(2000, 3, 1, -1, 0, TimeFormat.Short, "2/29/2000 12:00:00 AM")]
	[TestCase(2000, 2, 28, 1, 0, TimeFormat.Short, "2/29/2000 12:00:00 AM")]
	[TestCase(2001, 2, 28, 1, 0, TimeFormat.Short, "3/1/2001 12:00:00 AM")]
	[TestCase(2001, 3, 1, -1, 0, TimeFormat.Short, "2/28/2001 12:00:00 AM")]
	[TestCase(2000, 1, 1, 365, 0, TimeFormat.Short, "12/31/2000 12:00:00 AM")]
	[TestCase(2000, 1, 1, 366, 0, TimeFormat.Short, "1/1/2001 12:00:00 AM")]
	[TestCase(2001, 1, 1, 364, 0, TimeFormat.Short, "12/31/2001 12:00:00 AM")]
	[TestCase(2001, 1, 1, 365, 0, TimeFormat.Short, "1/1/2002 12:00:00 AM")]
	[TestCase(2100, 1, 1, 364, 0, TimeFormat.Short, "12/31/2100 12:00:00 AM")]
	[TestCase(2100, 1, 1, 365, 0, TimeFormat.Short, "1/1/2101 12:00:00 AM")]
	public void TestPointFormatWithOffset(int year, int month, int day, int dayOffset, int secondOffset, TimeFormat format,
		string expectedOutput)
	{
		var calendar = StandardCalendar.CreateStandard();
		var point = calendar.CreateTimePoint(year, month, day);
		var offset = calendar.CreateTimeOffset(dayOffset, 0, 0, secondOffset);
		var adjustedPoint = point + offset;
		Assert.That(calendar.FormatTime(adjustedPoint, format), Is.EqualTo(expectedOutput));
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
	[TestCase(2000, 1, 1, 31, TimeFormat.Long, "1 month")]
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
	[TestCase(2001, 1, 1, 31, TimeFormat.Long, "1 month")]
	[TestCase(2001, 1, 31, 32, TimeFormat.Long, "1 month, 4 days")]
	[TestCase(2001, 1, 31, 29, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2001, 2, 27, 32, TimeFormat.Long, "1 month, 4 days")]
	[TestCase(2001, 2, 27, 28, TimeFormat.Long, "1 month")]
	[TestCase(2001, 2, 28, 32, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2001, 2, 28, 27, TimeFormat.Long, "27 days")]
	[TestCase(2001, 12, 31, 32, TimeFormat.Long, "1 month, 1 day")]
	[TestCase(2001, 12, 31, 31, TimeFormat.Long, "1 month")]
	public void TestOffsetFromFormat(int year, int month, int day, int dayOffset, TimeFormat format, string expectedOutput)
	{
		var calendar = StandardCalendar.CreateStandard();
		var point = calendar.CreateTimePoint(year, month, day);
		var offset = calendar.CreateTimeOffset(dayOffset, 0, 0, 0);
		var output = calendar.FormatOffsetFrom(point, offset, format);
		Assert.That(output, Is.EqualTo(expectedOutput));
	}

	[Test]
	public void TestDayAfterEndOfYearInLeapCentury()
	{
		var calendar = StandardCalendar.CreateStandard();
		var point = calendar.CreateTimePoint(2103, 12, 31) + calendar.CreateTimeOffset(1, 0, 0, 0);
		Assert.That(calendar.FormatTime(point, TimeFormat.Short), Is.EqualTo("1/1/2104 12:00:00 AM"));
	}

	[Test]
	public void TestEveryDayMatchesDateTime()
	{
		var calendar = StandardCalendar.CreateStandard();
		var oneDay = calendar.CreateTimeOffset(1, 0, 0, 0);
		var point = calendar.CreateTimePoint(1600, 1, 1);
		for (var date = new DateTime(1600, 1, 1); date.Year <= 2400; date = date.AddDays(1))
		{
			var created = calendar.CreateTimePoint(date.Year, date.Month, date.Day);
			if (created.TotalSeconds != point.TotalSeconds)
				Assert.Fail($"Creating {date:d} gave a different time than adding days up to it.");

			var expected = date.ToString("G", CultureInfo.CurrentCulture);
			var actual = calendar.FormatTime(point, TimeFormat.Short);
			if (actual != expected)
				Assert.Fail($"Adding days up to {date:d} formatted as {actual}.");

			point += oneDay;
		}
	}

	[TestCase(24, 0, 0)]
	[TestCase(0, 60, 0)]
	[TestCase(0, 0, 60)]
	public void TestTimeOutOfRangeThrows(int hour, int minute, int second)
	{
		var calendar = StandardCalendar.CreateStandard();
		Assert.Throws<ArgumentException>(() => calendar.CreateTimePoint(2001, 1, 1, hour, minute, second));
	}

	[TestCase(2004, 1, 32)]
	[TestCase(2004, 4, 31)]
	[TestCase(2004, 12, 32)]
	public void TestDayOutOfRangeInLeapYearThrows(int year, int month, int day)
	{
		var calendar = StandardCalendar.CreateStandard();
		Assert.Throws<ArgumentException>(() => calendar.CreateTimePoint(year, month, day));
	}

	[Test]
	public void TestYearThatCannotBeFormattedThrows()
	{
		var calendar = StandardCalendar.CreateStandard();
		Assert.Throws<ArgumentException>(() => calendar.CreateTimePoint(0, 1, 1));
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

	[Test]
	public void TestSubtractingLaterPointIsNegative()
	{
		var calendar = StandardCalendar.CreateStandard();
		var earlier = calendar.CreateTimePoint(2001, 1, 1);
		var later = calendar.CreateTimePoint(2001, 1, 2);
		Assert.That((later + (earlier - later)).TotalSeconds, Is.EqualTo(earlier.TotalSeconds));
	}

	[Test]
	public void TestSubtractingPointsFromDifferentCalendarsThrows()
	{
		var point1 = StandardCalendar.CreateStandard().CreateTimePoint(2001, 1, 1);
		var point2 = StandardCalendar.CreateStandard().CreateTimePoint(2001, 1, 1);
		Assert.Throws<InvalidOperationException>(() => _ = point1 - point2);
	}

	[Test]
	public void TestTimePointEquality()
	{
		var calendar = StandardCalendar.CreateStandard();
		var point1 = calendar.CreateTimePoint(2001, 1, 1);
		var point2 = calendar.CreateTimePoint(2001, 1, 1);
		var point3 = calendar.CreateTimePoint(2001, 1, 2);
		Assert.Multiple(() =>
		{
			Assert.That(point1.Equals(point2), Is.True);
			Assert.That(point1 == point2, Is.True);
			Assert.That(point1 != point2, Is.False);
			Assert.That(point1.GetHashCode(), Is.EqualTo(point2.GetHashCode()));
			Assert.That(point1.Equals(point3), Is.False);
			Assert.That(point1 == point3, Is.False);
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
		});
	}
}
