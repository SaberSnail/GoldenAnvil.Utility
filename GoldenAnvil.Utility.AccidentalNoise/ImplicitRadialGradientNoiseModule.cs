using System;

namespace AccidentalNoise
{
	public sealed class ImplicitRadialGradientNoiseModule : ImplicitGradientNoiseModule
	{
		public ImplicitRadialGradientNoiseModule(GradientOverflow overflow, double x1, double y1, double centerX, double centerY, double radius)
			: base(overflow)
		{
			m_x1 = x1;
			m_y1 = y1;
			m_centerX = centerX;
			m_centerY = centerY;
			m_radius = radius;
		}

		protected override double GetValueCore(double x, double y)
		{
			double dx = x - m_x1;
			double dy = y - m_y1;
			double edgeX;
			double edgeY;

			if (dx == 0)
			{
				double B = -2.0 * m_centerY;
				double C = x * x - 2.0 * x * m_centerX + m_centerX * m_centerX + m_centerY * m_centerY - m_radius * m_radius;
				double disc = B * B - 4.0 * C;
				if (disc <= 0)
					return 0;
				edgeX = x;
				edgeY = y > m_y1 ? (-B + Math.Sqrt(disc)) / 2.0 : (-B - Math.Sqrt(disc)) / 2.0;
			}
			else
			{
				double m = dy / dx;
				double b = m_y1 - m * m_x1;
				double A = m * m + 1;
				double B = 2 * (m * b - m * m_centerY - m_centerX);
				double C = m_centerY * m_centerY - m_radius * m_radius + m_centerX * m_centerX - 2 * b * m_centerY + b * b;
				double disc = B * B - 4 * A * C;
				if (disc <= 0)
					return 0;
				edgeX = x > m_x1 ? (-B + Math.Sqrt(disc)) / (2 * A) : (-B - Math.Sqrt(disc)) / (2 * A);
				edgeY = m * edgeX + b;
			}

			double dXEdge = edgeX - m_x1;
			double dYEdge = edgeY - m_y1;
			return Math.Sqrt(dx * dx + dy * dy) / Math.Sqrt(dXEdge * dXEdge + dYEdge * dYEdge);
		}

		readonly double m_x1;
		readonly double m_y1;
		readonly double m_centerX;
		readonly double m_centerY;
		readonly double m_radius;
	}
}
