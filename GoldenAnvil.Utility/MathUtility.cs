using System;

namespace GoldenAnvil.Utility
{
	public static class MathUtility
	{
		public const double Sin30 = 0.5;
		public const double Sin60 = 0.8660254038;
		public const double Cos30 = 0.8660254038;
		public const double Cos60 = 0.5;

		public static double Clamp(double value, double min, double max)
		{
			return Math.Max(Math.Min(value, max), min);
		}
	}
}
