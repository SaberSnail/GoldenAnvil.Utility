using System;
using GoldenAnvil.Utility;
using JetBrains.Annotations;

namespace AccidentalNoise
{
	public sealed class ImplicitFractalNoiseModule : ImplicitNoiseModuleBase
	{
		public static ImplicitFractalNoiseModule Create(FractalType fractalType, BasisType basisType, InterpolationType interpolationType, int octaves, double frequency, double lacunarity, uint seed)
		{
			switch (fractalType)
			{
			case FractalType.Fbm:
				return new ImplicitFractalNoiseModule(fractalType, basisType, interpolationType, octaves, frequency, lacunarity, 0.5, 0, 1.0, seed);
			case FractalType.RidgedMulti:
				return new ImplicitFractalNoiseModule(fractalType, basisType, interpolationType, octaves, frequency, lacunarity, 0.5, 1, 0.9, seed);
			case FractalType.Billow:
				return new ImplicitFractalNoiseModule(fractalType, basisType, interpolationType, octaves, frequency, lacunarity, 0.5, 0, 1, seed);
			case FractalType.Multi:
				return new ImplicitFractalNoiseModule(fractalType, basisType, interpolationType, octaves, frequency, lacunarity, 0, 0, 1, seed);
			case FractalType.HybridMulti:
				return new ImplicitFractalNoiseModule(fractalType, basisType, interpolationType, octaves, frequency, lacunarity, 1, 0.7, 0.25, seed);
			case FractalType.DeCarpentierSwiss:
				return new ImplicitFractalNoiseModule(fractalType, basisType, interpolationType, octaves, frequency, lacunarity, 0.6, 0.15, 0.9, seed);
			default:
				throw new NotImplementedException();
			}
		}

		public ImplicitFractalNoiseModule(FractalType fractalType, BasisType basisType, InterpolationType interpolationType, int octaves, double frequency, double lacunarity, double gain, double offset, double h, uint seed)
		{
			if (octaves > MaxSources)
				throw new ArgumentOutOfRangeException("octaves ({0}) may not be larger than MaxSources ({1})".FormatInvariant(octaves, MaxSources));

			m_fractalType = fractalType;
			m_octaves = octaves;
			m_frequency = frequency;
			m_lacunarity = lacunarity;
			m_gain = gain;
			m_offset = offset;
			m_h = h;

			m_expArray = new double[m_octaves];
			m_correct = new double[m_octaves,2];
			m_basis = new ImplicitBasisFunctionNoiseModule[m_octaves];
			for (uint i = 0; i < m_octaves; i++)
				m_basis[i] = new ImplicitBasisFunctionNoiseModule(basisType, interpolationType, seed + i * 300);

			CalculateWeights();
		}

		public ImplicitFractalNoiseModule(FractalType fractalType, [NotNull] ImplicitBasisFunctionNoiseModule[] basis, double frequency, double lacunarity, double gain, double offset, double h)
		{
			if (basis == null)
				throw new ArgumentNullException("basis");
			if (basis.Length == 0)
				throw new ArgumentException("basis count ({0}) must be at least 1".FormatInvariant(basis.Length));
			if (basis.Length > MaxSources)
				throw new ArgumentOutOfRangeException("basis count ({0}) may not be larger than MaxSources ({1})".FormatInvariant(basis.Length, MaxSources));

			m_fractalType = fractalType;
			m_octaves = basis.Length;
			m_frequency = frequency;
			m_lacunarity = lacunarity;
			m_gain = gain;
			m_offset = offset;
			m_h = h;

			m_expArray = new double[m_octaves];
			m_correct = new double[m_octaves, 2];
			m_basis = basis;

			CalculateWeights();
		}

		public override double GetValue(double x, double y)
		{
			switch (m_fractalType)
			{
			case FractalType.Fbm:
				return GetValueForFbm(x, y);
			case FractalType.RidgedMulti:
				return GetValueForRidgedMulti(x, y);
			case FractalType.Billow:
				return GetValueForBillow(x, y);
			case FractalType.Multi:
				return GetValueForMulti(x, y);
			case FractalType.HybridMulti:
				return GetValueForHybridMulti(x, y);
			case FractalType.DeCarpentierSwiss:
				return GetValueForDeCarpentierSwiss(x, y);
			default:
				throw new NotImplementedException();
			}
		}

		private void CalculateWeights()
		{
			switch (m_fractalType)
			{
			case FractalType.Fbm:
				CalculateWeightsForFbm();
				break;
			case FractalType.RidgedMulti:
				CalculateWeightsForRidgedMulti();
				break;
			case FractalType.Billow:
				CalculateWeightsForBillow();
				break;
			case FractalType.Multi:
				CalculateWeightsForMulti();
				break;
			case FractalType.HybridMulti:
				CalculateWeightsForHybridMulti();
				break;
			case FractalType.DeCarpentierSwiss:
				CalculateWeightsForDeCarpentierSwiss();
				break;
			default:
				throw new NotImplementedException();
			}
		}

		private void CalculateWeightsForFbm()
		{
			for (int i = 0; i < m_octaves; i++)
				m_expArray[i] = Math.Pow(m_lacunarity, -i * m_h);

			// Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
			double minValue = 0.0;
			double maxValue = 0.0;
			for (int i = 0; i < m_octaves; i++)
			{
				minValue += -1.0 * m_expArray[i];
				maxValue += 1.0 * m_expArray[i];

				const double a = -1.0;
				const double b = 1.0;
				double scale = (b - a) / (maxValue - minValue);
				double bias = a - minValue * scale;
				m_correct[i, 0] = scale;
				m_correct[i, 1] = bias;
			}
		}

		private double GetValueForFbm(double x, double y)
		{
			double sum = 0;
			double amp = 1.0;

			x *= m_frequency;
			y *= m_frequency;

			for (int i = 0; i < m_octaves; i++)
			{
				double n = m_basis[i].GetValue(x, y);
				sum += n * amp;
				amp *= m_gain;

				x *= m_lacunarity;
				y *= m_lacunarity;
			}

			return sum;
		}

		private void CalculateWeightsForRidgedMulti()
		{
			for (int i = 0; i < m_octaves; i++)
				m_expArray[i] = Math.Pow(m_lacunarity, -i * m_h);

			// Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
			double minValue = 0.0;
			double maxValue = 0.0;
			for (int i = 0; i < m_octaves; i++)
			{
				minValue += (m_offset - 1.0) * (m_offset - 1.0) * m_expArray[i];
				maxValue += (m_offset) * (m_offset) * m_expArray[i];

				const double a = -1.0;
				const double b = 1.0;
				double scale = (b - a) / (maxValue - minValue);
				double bias = a - minValue * scale;
				m_correct[i, 0] = scale;
				m_correct[i, 1] = bias;
			}
		}

		private double GetValueForRidgedMulti(double x, double y)
		{
			double sum = 0;
			double amp = 1.0;

			x *= m_frequency;
			y *= m_frequency;

			for (uint i = 0; i < m_octaves; ++i)
			{
				double n = m_basis[i].GetValue(x, y);
				n = 1.0 - Math.Abs(n);
				sum += amp * n;
				amp *= m_gain;

				x *= m_lacunarity;
				y *= m_lacunarity;
			}
			return sum;
		}

		private void CalculateWeightsForBillow()
		{
			for (int i = 0; i < m_octaves; i++)
				m_expArray[i] = Math.Pow(m_lacunarity, -i * m_h);

			// Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
			double minValue = 0.0;
			double maxValue = 0.0;
			for (int i = 0; i < m_octaves; i++)
			{
				minValue += -1.0 * m_expArray[i];
				maxValue += 1.0 * m_expArray[i];

				const double a = -1.0;
				const double b = 1.0;
				double scale = (b - a) / (maxValue - minValue);
				double bias = a - minValue * scale;
				m_correct[i, 0] = scale;
				m_correct[i, 1] = bias;
			}
		}

		private double GetValueForBillow(double x, double y)
		{
			double sum = 0.0;
			double amp = 1.0;

			x *= m_frequency;
			y *= m_frequency;

			for (uint i = 0; i < m_octaves; ++i)
			{
				double n = m_basis[i].GetValue(x, y);
				sum += (2.0 * Math.Abs(n) - 1.0) * amp;
				amp *= m_gain;

				x *= m_lacunarity;
				y *= m_lacunarity;
			}
			return sum;
		}

		private void CalculateWeightsForMulti()
		{
			for (int i = 0; i < m_octaves; i++)
				m_expArray[i] = Math.Pow(m_lacunarity, -i * m_h);

			// Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
			double minValue = 1.0;
			double maxValue = 1.0;
			for (int i = 0; i < m_octaves; i++)
			{
				minValue *= -1.0 * m_expArray[i] + 1.0;
				maxValue *= 1.0 * m_expArray[i] + 1.0;

				const double a = -1.0;
				const double b = 1.0;
				double scale = (b - a) / (maxValue - minValue);
				double bias = a - minValue * scale;
				m_correct[i, 0] = scale;
				m_correct[i, 1] = bias;
			}
		}

		private double GetValueForMulti(double x, double y)
		{
			double value = 1.0;
			x *= m_frequency;
			y *= m_frequency;

			for (uint i = 0; i < m_octaves; ++i)
			{
				value *= m_basis[i].GetValue(x, y) * m_expArray[i] + 1.0;
				x *= m_lacunarity;
				y *= m_lacunarity;
			}

			return value * m_correct[m_octaves - 1, 0] + m_correct[m_octaves - 1, 1];
		}

		private void CalculateWeightsForHybridMulti()
		{
			for (int i = 0; i < m_octaves; ++i)
				m_expArray[i] = Math.Pow(m_lacunarity, -i * m_h);

			// Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
			const double a = -1.0;
			const double b = 1.0;

			double minValue = m_offset - 1.0;
			double maxValue = m_offset + 1.0;
			double weightMin = m_gain * minValue;
			double weightMax = m_gain * maxValue;

			double scale = (b - a) / (maxValue - minValue);
			double bias = a - minValue * scale;
			m_correct[0, 0] = scale;
			m_correct[0, 1] = bias;


			for (int i = 1; i < m_octaves; i++)
			{
				if (weightMin > 1.0)
					weightMin = 1.0;
				if (weightMax > 1.0)
					weightMax = 1.0;

				double signal = (m_offset - 1.0) * m_expArray[i];
				minValue += signal * weightMin;
				weightMin *= m_gain * signal;

				signal = (m_offset + 1.0) * m_expArray[i];
				maxValue += signal * weightMax;
				weightMax *= m_gain * signal;

				scale = (b - a) / (maxValue - minValue);
				bias = a - minValue * scale;
				m_correct[i, 0] = scale;
				m_correct[i, 1] = bias;
			}
		}

		private double GetValueForHybridMulti(double x, double y)
		{
			x *= m_frequency;
			y *= m_frequency;

			double value = m_basis[0].GetValue(x, y) + m_offset;
			double weight = m_gain * value;
			x *= m_lacunarity;
			y *= m_lacunarity;

			for (uint i = 1; i < m_octaves; ++i)
			{
				if (weight > 1.0) weight = 1.0;
				double signal = (m_basis[i].GetValue(x, y) + m_offset) * m_expArray[i];
				value += weight * signal;
				weight *= m_gain * signal;
				x *= m_lacunarity;
				y *= m_lacunarity;
			}

			return value * m_correct[m_octaves - 1, 0] + m_correct[m_octaves - 1, 1];
		}

		private void CalculateWeightsForDeCarpentierSwiss()
		{
			for (int i = 0; i < m_octaves; i++)
				m_expArray[i] = Math.Pow(m_lacunarity, -i * m_h);

			// Calculate scale/bias pairs by guessing at minimum and maximum values and remapping to [-1,1]
			double minValue = 0.0;
			double maxValue = 0.0;
			for (int i = 0; i < m_octaves; i++)
			{
				minValue += (m_offset - 1.0) * (m_offset - 1.0) * m_expArray[i];
				maxValue += (m_offset) * (m_offset) * m_expArray[i];

				const double a = -1.0;
				const double b = 1.0;
				double scale = (b - a) / (maxValue - minValue);
				double bias = a - minValue * scale;
				m_correct[i, 0] = scale;
				m_correct[i, 1] = bias;
			}
		}

		private double GetValueForDeCarpentierSwiss(double x, double y)
		{
			double sum = 0;
			double amp = 1.0;

			double dx_sum = 0;
			double dy_sum = 0;

			x *= m_frequency;
			y *= m_frequency;

			for (uint i = 0; i < m_octaves; ++i)
			{
				double n = m_basis[i].GetValue(x + m_offset * dx_sum, y + m_offset * dy_sum);
				double dx = m_basis[i].GetDX(x + m_offset * dx_sum, y + m_offset * dy_sum);
				double dy = m_basis[i].GetDY(x + m_offset * dx_sum, y + m_offset * dy_sum);
				sum += amp * (1.0 - Math.Abs(n));
				dx_sum += amp * dx * -n;
				dy_sum += amp * dy * -n;
				amp *= m_gain * MathUtility.Clamp(sum, 0.0, 1.0);
				x *= m_lacunarity;
				y *= m_lacunarity;
			}
			return sum;
		}

		readonly FractalType m_fractalType;
		readonly int m_octaves;
		readonly double m_frequency;
		readonly double m_lacunarity;
		readonly double m_gain;
		readonly double m_offset;
		readonly double m_h;
		readonly ImplicitBasisFunctionNoiseModule[] m_basis;
		readonly double[] m_expArray;
		readonly double[,] m_correct;
	}
}
