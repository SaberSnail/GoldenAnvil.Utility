namespace GoldenAnvil.Utility.Calendar;

/// <summary>
/// The length of a <see cref="TimeOffset"/> in days, hours, minutes, and seconds, as its calendar divides it.
/// Every part of a negative offset is zero or negative.
/// </summary>
public readonly record struct TimeOffsetParts(long Days, int Hours, int Minutes, int Seconds);
