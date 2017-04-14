namespace AccidentalNoise
{
	public sealed class ImplicitConstantNoiseModule : ImplicitNoiseModuleBase
	{
		public ImplicitConstantNoiseModule(double value)
		{
			m_value = value;
		}

		public override double GetValue(double x, double y)
		{
			return m_value;
		}

		readonly double m_value;
	}
}
