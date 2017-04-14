using System;
using GoldenAnvil.Utility;
using JetBrains.Annotations;

namespace AccidentalNoise
{
	// This module attempts to correct the range of its input to fall within a desired output range.
	// Operates by sampling the input function a number of times across an section of the domain, and using the
	// calculated min/max values to generate scale/offset pair to apply to the input. The calculate() function performs
	// this sampling and calculation, and is called automatically whenever a source is set, or whenever the desired output
	// ranges are set. Also, the function may be called manually, if desired.
	public sealed class ImplicitAutoCorrectNoiseModule : ImplicitNoiseModuleBase
	{
		public ImplicitAutoCorrectNoiseModule([NotNull] ImplicitNoiseModuleBase source, double low, double high)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			m_source = source;
			m_low = low;
			m_high = high;
			Initialize();
		}

		public override double GetValue(double x, double y)
		{
			double v = m_source.GetValue(x, y);
			return MathUtility.Clamp(v * m_scale2D + m_offset2D, m_low, m_high);
		}

		private void Initialize()
		{
			RandomLCG lcg = new RandomLCG(10000);

			// Calculate 2D
			double mn = 10000.0;
			double mx = -10000.0;
			for (int c = 0; c < 10000; ++c)
			{
				double nx = lcg.Next01() * 4.0 - 2.0;
				double ny = lcg.Next01() * 4.0 - 2.0;

				double v = m_source.GetValue(nx, ny);
				if (v < mn) mn = v;
				if (v > mx) mx = v;
			}
			m_scale2D = (m_high - m_low) / (mx - mn);
			m_offset2D = m_low - mn * m_scale2D;
		}

		readonly ImplicitNoiseModuleBase m_source;
		readonly double m_low;
		readonly double m_high;
		double m_scale2D;
		double m_offset2D;
	}
}
