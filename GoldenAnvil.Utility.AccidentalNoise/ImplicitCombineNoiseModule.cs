using System;
using GoldenAnvil.Utility;
using JetBrains.Annotations;

namespace AccidentalNoise
{
	public sealed class ImplicitCombineNoiseModule : ImplicitNoiseModuleBase
	{
		public ImplicitCombineNoiseModule(CombineOperationType operation, [NotNull] ImplicitNoiseModuleBase[] sources)
		{
			if (sources == null)
				throw new ArgumentNullException("sources");
			if (sources.Length < 2)
				throw new ArgumentException("sources count ({0}) must be at least 2".FormatInvariant(sources.Length));
			if (sources.Length > MaxSources)
				throw new ArgumentOutOfRangeException("sources count ({0}) may not be larger than MaxSources ({1})".FormatInvariant(sources.Length, MaxSources));

			m_operation = operation;
			m_sources = sources;
		}

		public override double GetValue(double x, double y)
		{
			switch (m_operation)
			{
			case CombineOperationType.Add:
				return GetAddValue(x, y);
			case CombineOperationType.Multiply:
				return GetMultiplyValue(x, y);
			case CombineOperationType.Maximum:
				return GetMaximumValue(x, y);
			case CombineOperationType.Minimum:
				return GetMinimumValue(x, y);
			case CombineOperationType.Average:
				return GetAverageValue(x, y);
			default:
				throw new NotImplementedException();
			}
		}

		private double GetAddValue(double x, double y)
		{
			double value = 0.0;
			foreach (ImplicitNoiseModuleBase source in m_sources)
				value += source.GetValue(x, y);
			return value;
		}

		private double GetMultiplyValue(double x, double y)
		{
			double value = 1.0;
			foreach (ImplicitNoiseModuleBase source in m_sources)
				value *= source.GetValue(x, y);
			return value;
		}

		private double GetMaximumValue(double x, double y)
		{
			double maxValue = double.NegativeInfinity;
			foreach (ImplicitNoiseModuleBase source in m_sources)
			{
				double value = source.GetValue(x, y);
				if (value > maxValue)
					maxValue = value;
			}
			return maxValue;
		}

		private double GetMinimumValue(double x, double y)
		{
			double minValue = double.PositiveInfinity;
			foreach (ImplicitNoiseModuleBase source in m_sources)
			{
				double value = source.GetValue(x, y);
				if (value < minValue)
					minValue = value;
			}
			return minValue;
		}

		private double GetAverageValue(double x, double y)
		{
			double value = 0.0;
			foreach (ImplicitNoiseModuleBase source in m_sources)
				value += source.GetValue(x, y);
			return value / m_sources.Length;
		}

		readonly CombineOperationType m_operation;
		readonly ImplicitNoiseModuleBase[] m_sources;
	}
}
