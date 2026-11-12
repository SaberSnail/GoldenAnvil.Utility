namespace GoldenAnvil.Utility.Calendar;

/// <summary>
/// The date and time of a <see cref="TimePoint"/>, as its calendar divides it.
/// </summary>
public readonly record struct TimePointParts(int Year, int Month, int Day, int Hour, int Minute, int Second);
