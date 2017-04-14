using System;

namespace AccidentalNoise
{
	public sealed class ImplicitConicalGradientNoiseModule : ImplicitGradientNoiseModule
	{
		public ImplicitConicalGradientNoiseModule(GradientOverflow overflow, double x1, double y1, double offset, double scale)
			: base(overflow)
		{
			m_x1 = x1;
			m_y1 = y1;
			m_offset = Math.PI + (-offset * 2.0 * Math.PI);
			m_scale = 1.0 / (2.0 * Math.PI * scale);
		}

		protected override double GetValueCore(double x, double y)
		{
			double dx = x - m_x1;
			double dy = y - m_y1;
			return (Math.Atan2(dy, dx) + m_offset) * m_scale;
		}

		readonly double m_x1;
		readonly double m_y1;
		readonly double m_offset;
		readonly double m_scale;
	}
}
