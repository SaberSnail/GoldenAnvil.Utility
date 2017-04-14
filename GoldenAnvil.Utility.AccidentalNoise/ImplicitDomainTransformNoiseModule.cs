using System;
using JetBrains.Annotations;

namespace AccidentalNoise
{
	public sealed class ImplicitDomainTransformNoiseModule : ImplicitNoiseModuleBase
	{
		public ImplicitDomainTransformNoiseModule([NotNull] ImplicitNoiseModuleBase source, [NotNull] ImplicitNoiseModuleBase xScale, [NotNull] ImplicitNoiseModuleBase xOffset, [NotNull] ImplicitNoiseModuleBase yScale, [NotNull] ImplicitNoiseModuleBase yOffset)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (xScale == null)
				throw new ArgumentNullException("xScale");
			if (xOffset == null)
				throw new ArgumentNullException("xOffset");
			if (yScale == null)
				throw new ArgumentNullException("yScale");
			if (yOffset == null)
				throw new ArgumentNullException("yOffset");

			m_source = source;
			m_xScale = xScale;
			m_xOffset = xOffset;
			m_yScale = yScale;
			m_yOffset = yOffset;
		}

		public override double GetValue(double x, double y)
		{
			return m_source.GetValue(x * m_xScale.GetValue(x, y) + m_xOffset.GetValue(x, y), y * m_yScale.GetValue(x, y) + m_yOffset.GetValue(x, y));
		}

		readonly ImplicitNoiseModuleBase m_source;
		readonly ImplicitNoiseModuleBase m_xScale;
		readonly ImplicitNoiseModuleBase m_xOffset;
		readonly ImplicitNoiseModuleBase m_yScale;
		readonly ImplicitNoiseModuleBase m_yOffset;
	}
}
