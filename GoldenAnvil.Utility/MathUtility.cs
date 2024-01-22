using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace GoldenAnvil.Utility
{
	public static class MathUtility
	{
		public const double Sin30 = 0.5;
		public const double Sin60 = 0.8660254038;
		public const double Cos30 = 0.8660254038;
		public const double Cos60 = 0.5;

		public static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;

		public static double Clamp(double value, double min, double max) => Math.Max(Math.Min(value, max), min);

		public static T Max<T>(params T[] values) where T : INumber<T> => values.Max();

		public static T Min<T>(params T[] values) where T : INumber<T> => values.Min();

		public static IReadOnlyList<double> SolveQuadratic(double a, double b, double c)
		{
			var values = new List<double>();

			var discriminant = (b * b) - (4 * a * c);
			if (discriminant == 0.0)
			{
				values.Add(-b / (2 * a));
			}
			else if (discriminant > 0)
			{
				var sqrtOfDiscriminant = Math.Sqrt(discriminant);
				values.Add((-b + sqrtOfDiscriminant) / (2 * a));
				values.Add((-b - sqrtOfDiscriminant) / (2 * a));
			}

			return values;
		}

		public static double NormalizeRadians(double angle)
		{
			var result = angle % (2.0 * Math.PI);
			if (result < 0.0)
				result += 2.0 * Math.PI;
			return result;
		}
	}
}
