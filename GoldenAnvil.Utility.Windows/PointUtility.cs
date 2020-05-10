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

		public static double SlopeTo(this Point point, Point target)
		{
			return (target.Y - point.Y) / (target.X - point.X);
		}

		public static double AngleTo(this Point point, Point target)
		{
			return Math.Atan2(target.Y - point.Y, target.X - point.X);
		}

		public static Vector VectorTo(this Point point, Point target)
		{
			return new Vector(target.X - point.X, target.Y - point.Y);
		}

		public static Point NearestPointOnLine(this Point point, Point target1, Point target2)
		{
			if (target1.X == target2.X)
				return new Point(target1.X, point.Y);

			var slope = target1.SlopeTo(target2);
			var offset = target1.Y - (slope * target1.X);

			var x = (point.X + slope * (point.Y - offset)) / (1.0 + (slope * slope));
			var y = ((slope * point.X) + (slope * slope * point.Y) + slope) / (1 + (slope * slope));
			return new Point(x, y);
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
