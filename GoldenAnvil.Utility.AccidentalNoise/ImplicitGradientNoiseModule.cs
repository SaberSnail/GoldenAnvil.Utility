using System;
using GoldenAnvil.Utility;

namespace AccidentalNoise
{
	public abstract class ImplicitGradientNoiseModule : ImplicitNoiseModuleBase
	{
		protected ImplicitGradientNoiseModule(GradientOverflow overflow)
		{
			m_overflow = overflow;
		}

		public override double GetValue(double x, double y)
		{
			double value = GetValueCore(x, y);
			return AdjustForOverflow(value);
		}

		protected abstract double GetValueCore(double x, double y);

		private double AdjustForOverflow(double value)
		{
			switch (m_overflow)
			{
			case GradientOverflow.None:
				return value;
			case GradientOverflow.Truncate:
				return MathUtility.Clamp(value, 0, 1);
			case GradientOverflow.Repeat:
				return value % 1.0;
			case GradientOverflow.Reflect:
				value = Math.Abs(value);
				if ((int) value % 2 == 0)
					return value % 1.0;
				else
					return 1.0 - value % 1.0;
			default:
				throw new NotImplementedException();
			}
		}

		readonly GradientOverflow m_overflow;
	}
}
