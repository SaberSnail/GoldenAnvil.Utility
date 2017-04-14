using System;
using JetBrains.Annotations;

namespace AccidentalNoise
{
	public sealed class ImplicitValueTransformNoiseModule : ImplicitNoiseModuleBase
	{
		public ImplicitValueTransformNoiseModule([NotNull] ImplicitNoiseModuleBase source, [NotNull] ImplicitNoiseModuleBase scale, [NotNull] ImplicitNoiseModuleBase offset)
		{
			if (source == null)
				throw new ArgumentNullException("source");
			if (scale == null)
				throw new ArgumentNullException("scale");
			if (offset == null)
				throw new ArgumentNullException("offset");

			m_source = source;
			m_scale = scale;
			m_offset = offset;
		}

		public override double GetValue(double x, double y)
		{
			return m_source.GetValue(x, y) * m_scale.GetValue(x, y) + m_offset.GetValue(x, y);
		}

		readonly ImplicitNoiseModuleBase m_source;
		readonly ImplicitNoiseModuleBase m_scale;
		readonly ImplicitNoiseModuleBase m_offset;
	}
}
