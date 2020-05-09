using System;
using System.Windows;

namespace GoldenAnvil.Utility.Windows
{
	public static class PointUtility
	{
		public static Point WithOffset(this Point point, Point that)
		{
			return new Point(point.X + that.X, point.Y + that.Y);
		}

		public static double DistanceTo(this Point point, Point target)
		{
			var dx = point.X - target.X;
			var dy = point.Y - target.Y;
			return Math.Sqrt((dx * dx) + (dy * dy));
		}

		public static Vector VectorTo(this Point point, Point target)
		{
			return new Vector(target.X - point.X, target.Y - point.Y);
		}

		public static Point RotateAround(this Point point, Point center, double angleInRadians)
		{
			var cosA = Math.Cos(angleInRadians);
			var sinA = Math.Sin(angleInRadians);

			var x = point.X - center.X;
			var y = point.Y - center.Y;

			x = x * cosA - y * sinA;
			y = x * sinA + y * cosA;

			return new Point(x + center.X, y + center.Y);
		}
	}
}
