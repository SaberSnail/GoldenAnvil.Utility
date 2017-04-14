namespace AccidentalNoise
{
	public sealed class ImplicitLinearGradientNoiseModule : ImplicitGradientNoiseModule
	{
		public ImplicitLinearGradientNoiseModule(GradientOverflow overflow, double x1, double y1, double x2, double y2)
			: base(overflow)
		{
			m_x1 = x1;
			m_y1 = y1;
			m_deltaX = x2 - x1;
			m_deltaY = y2 - y1;
			m_length = m_deltaX * m_deltaX + m_deltaY * m_deltaY;
		}

		protected override double GetValueCore(double x, double y)
		{
			double dx = x - m_x1;
			double dy = y - m_y1;
			return (dx * m_deltaX + dy * m_deltaY) / m_length;
		}

		readonly double m_x1;
		readonly double m_y1;
		readonly double m_deltaX;
		readonly double m_deltaY;
		readonly double m_length;
	}
}