using System;
using JetBrains.Annotations;

namespace AccidentalNoise
{
	public sealed class ImplicitCacheNoiseModule : ImplicitNoiseModuleBase
	{
		public ImplicitCacheNoiseModule([NotNull] ImplicitNoiseModuleBase source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			m_source = source;
			m_x = double.PositiveInfinity;
		}

		public override double GetValue(double x, double y)
		{
			if (m_x == x && m_y == y)
				return m_value;
			m_x = x;
			m_y = y;
			m_value = m_source.GetValue(x, y);
			return m_value;
		}

		readonly ImplicitNoiseModuleBase m_source;
		double m_value;
		double m_x;
		double m_y;
	}
}
