using System;
using System.Windows;

namespace GoldenAnvil.Utility.Windows
{
	public static class PointUtility
	{
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
	}
}
