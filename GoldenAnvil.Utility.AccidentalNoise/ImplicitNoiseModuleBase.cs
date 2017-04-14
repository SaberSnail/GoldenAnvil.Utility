namespace AccidentalNoise
{
	public abstract class ImplicitNoiseModuleBase
	{
		protected ImplicitNoiseModuleBase()
		{
			m_spacing = 0.0001;
		}

		public abstract double GetValue(double x, double y);

		public double GetDX(double x, double y)
		{
			return (GetValue(x - m_spacing, y) - GetValue(x + m_spacing, y)) / m_spacing;
		}

		public double GetDY(double x, double y)
		{
			return (GetValue(x, y - m_spacing) - GetValue(x, y + m_spacing)) / m_spacing;
		}

		public static implicit operator ImplicitNoiseModuleBase(double value)
		{
			return new ImplicitConstantNoiseModule(value);
		}

		protected const int MaxSources = 20;

		readonly double m_spacing;
	}
}
