namespace GoldenAnvil.Utility.Calendar;

/// <summary>
/// The length of a <see cref="TimeOffset"/> in a <see cref="StandardCalendar"/>, in days, hours, minutes, and seconds.
/// Every part of a negative offset is zero or negative.
/// </summary>
public readonly record struct StandardTimeOffsetParts(long Days, int Hours, int Minutes, int Seconds);
