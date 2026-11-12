namespace GoldenAnvil.Utility.Calendar;

/// <summary>
/// The date and time of a <see cref="TimePoint"/> in a <see cref="StandardCalendar"/>.
/// </summary>
public readonly record struct StandardTimePointParts(int Year, int Month, int Day, int Hour, int Minute, int Second);
