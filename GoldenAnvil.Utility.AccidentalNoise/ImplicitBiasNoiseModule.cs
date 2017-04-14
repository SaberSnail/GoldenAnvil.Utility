using System;
using JetBrains.Annotations;

namespace AccidentalNoise
{
	public sealed class ImplicitBiasNoiseModule : ImplicitNoiseModuleBase
	{
		public ImplicitBiasNoiseModule([NotNull] ImplicitNoiseModuleBase source, [NotNull] ImplicitNoiseModuleBase bias)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (bias == null)
				throw new ArgumentNullException("bias");

			m_source = source;
			m_bias = bias;
		}

		public override double GetValue(double x, double y)
		{
			return NoiseUtility.Bias(m_bias.GetValue(x, y), m_source.GetValue(x, y));
		}

		readonly ImplicitNoiseModuleBase m_source;
		readonly ImplicitNoiseModuleBase m_bias;
	}
}
