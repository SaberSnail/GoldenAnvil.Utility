using System;
using JetBrains.Annotations;

namespace AccidentalNoise
{
	public sealed class ImplicitSelectNoiseModule : ImplicitNoiseModuleBase
	{
		public ImplicitSelectNoiseModule([NotNull] ImplicitNoiseModuleBase controlSource, [NotNull] ImplicitNoiseModuleBase lowSource, [NotNull] ImplicitNoiseModuleBase highSource, [NotNull] ImplicitNoiseModuleBase threshold, [NotNull] ImplicitNoiseModuleBase falloff)
		{
			if (controlSource == null)
				throw new ArgumentNullException("controlSource");
			if (lowSource == null)
				throw new ArgumentNullException("lowSource");
			if (highSource == null)
				throw new ArgumentNullException("highSource");
			if (threshold == null)
				throw new ArgumentNullException("threshold");
			if (falloff == null)
				throw new ArgumentNullException("falloff");

			m_controlSource = controlSource;
			m_lowSource = lowSource;
			m_highSource = highSource;
			m_threshold = threshold;
			m_falloff = falloff;
		}

		public override double GetValue(double x, double y)
		{
			double controlValue = m_controlSource.GetValue(x, y);
			double falloffValue = m_falloff.GetValue(x, y);
			double thresholdValue = m_threshold.GetValue(x, y);
			double value;

			if (falloffValue > 0.0)
			{
				if (controlValue < (thresholdValue - falloffValue))
				{
					value = m_lowSource.GetValue(x, y);
				}
				else if (controlValue > (thresholdValue + falloffValue))
				{
					value = m_highSource.GetValue(x, y);
				}
				else
				{
					double lower = thresholdValue - falloffValue;
					double upper = thresholdValue + falloffValue;
					double blend = NoiseUtility.QuinticInterpolation((controlValue - lower) / (upper - lower));
					return NoiseUtility.Lerp(blend, m_lowSource.GetValue(x, y), m_highSource.GetValue(x, y));
				}
			}
			else
			{
				value = controlValue < thresholdValue ? m_lowSource.GetValue(x, y) : m_highSource.GetValue(x, y);
			}

			return value;
		}

		readonly ImplicitNoiseModuleBase m_controlSource;
		readonly ImplicitNoiseModuleBase m_lowSource;
		readonly ImplicitNoiseModuleBase m_highSource;
		readonly ImplicitNoiseModuleBase m_threshold;
		readonly ImplicitNoiseModuleBase m_falloff;
	}
}
