using System;
using InterpolationFunc = System.Func<double, double>;
using NoiseFunc = System.Func<double, double, uint, System.Func<double, double>, double>;

namespace AccidentalNoise
{
	public sealed class ImplicitBasisFunctionNoiseModule : ImplicitNoiseModuleBase
	{
		public ImplicitBasisFunctionNoiseModule(BasisType basisType, InterpolationType interpolationType, uint seed)
		{
			m_basisType = basisType;
			m_interpolationType = interpolationType;
			m_seed = seed;
			m_rotMatrix = new double[3, 3];
			m_scale = new double[4];
			m_offset = new double[4];
			Initialize();
		}

		private void Initialize()
		{
			RandomLCG lcg = new RandomLCG(m_seed);
			double ax;
			double ay;
			double az;
			double len;

			ax = lcg.Next01();
			ay = lcg.Next01();
			az = lcg.Next01();
			len = Math.Sqrt(ax * ax + ay * ay + az * az);
			ax /= len;
			ay /= len;
			az /= len;
			SetRotationAngle(ax, ay, az, lcg.Next01() * Math.PI * 2.0);
			double angle = lcg.Next01() * Math.PI * 2.0;
			m_cos2d = Math.Cos(angle);
			m_sin2d = Math.Sin(angle);

			switch (m_basisType)
			{
			case BasisType.Value:
				m_noiseFunc = NoiseUtility.GetValueNoise2D;
				break;
			case BasisType.Gradient:
				m_noiseFunc = NoiseUtility.GetGradientNoise2D;
				break;
			case BasisType.GradientValue:
				m_noiseFunc = NoiseUtility.GetGradientValueNoise2D;
				break;
			case BasisType.White:
				m_noiseFunc = NoiseUtility.GetWhiteNoise2D;
				break;
			case BasisType.Simplex:
				m_noiseFunc = NoiseUtility.GetSimplexNoise2D;
				break;
			}

			switch (m_interpolationType)
			{
			case InterpolationType.None:
				m_interpolationFunc = NoiseUtility.NoInterpolation;
				break;
			case InterpolationType.Linear:
				m_interpolationFunc = NoiseUtility.LinearInterpolation;
				break;
			case InterpolationType.Cubic:
				m_interpolationFunc = NoiseUtility.HermiteInterpolation;
				break;
			case InterpolationType.Quintic:
				m_interpolationFunc = NoiseUtility.QuinticInterpolation;
				break;
			}

			SetMagicNumbers();
		}

		public override double GetValue(double x, double y)
		{
			double nx = x * m_cos2d - y * m_sin2d;
			double ny = y * m_cos2d + x * m_sin2d;
			return m_noiseFunc(nx, ny, m_seed, m_interpolationFunc);
		}

		private void SetMagicNumbers()
		{
			// This function is a damned hack.
			// The underlying noise functions don't return values in the range [-1,1] cleanly, and the ranges vary depending
			// on basis type and dimensionality. There's probably a better way to correct the ranges, but for now I'm just
			// setting he magic numbers m_scale and m_offset manually to empirically determined magic numbers.
			switch (m_basisType)
			{
			case BasisType.Value:
				m_scale[0] = 1.0;
				m_offset[0] = 0.0;
				m_scale[1] = 1.0;
				m_offset[1] = 0.0;
				m_scale[2] = 1.0;
				m_offset[2] = 0.0;
				m_scale[3] = 1.0;
				m_offset[3] = 0.0;
				break;

			case BasisType.Gradient:
				m_scale[0] = 1.86848;
				m_offset[0] = -0.000118;
				m_scale[1] = 1.85148;
				m_offset[1] = -0.008272;
				m_scale[2] = 1.64127;
				m_offset[2] = -0.01527;
				m_scale[3] = 1.92517;
				m_offset[3] = 0.03393;
				break;

			case BasisType.GradientValue:
				m_scale[0] = 0.6769;
				m_offset[0] = -0.00151;
				m_scale[1] = 0.6957;
				m_offset[1] = -0.133;
				m_scale[2] = 0.74622;
				m_offset[2] = 0.01916;
				m_scale[3] = 0.7961;
				m_offset[3] = -0.0352;
				break;

			case BasisType.White:
				m_scale[0] = 1.0;
				m_offset[0] = 0.0;
				m_scale[1] = 1.0;
				m_offset[1] = 0.0;
				m_scale[2] = 1.0;
				m_offset[2] = 0.0;
				m_scale[3] = 1.0;
				m_offset[3] = 0.0;
				break;

			case BasisType.Simplex:
				m_scale[0] = 1.0;
				m_offset[0] = 0.0;
				m_scale[1] = 1.0;
				m_offset[1] = 0.0;
				m_scale[2] = 1.0;
				m_offset[2] = 0.0;
				m_scale[3] = 1.0;
				m_offset[3] = 0.0;
				break;
			}
		}

		private void SetRotationAngle(double x, double y, double z, double angle)
		{
			m_rotMatrix[0, 0] = 1 + (1 - Math.Cos(angle)) * (x * x - 1);
			m_rotMatrix[1, 0] = -z * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * y;
			m_rotMatrix[2, 0] = y * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * z;

			m_rotMatrix[0, 1] = z * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * y;
			m_rotMatrix[1, 1] = 1 + (1 - Math.Cos(angle)) * (y * y - 1);
			m_rotMatrix[2, 1] = -x * Math.Sin(angle) + (1 - Math.Cos(angle)) * y * z;

			m_rotMatrix[0, 2] = -y * Math.Sin(angle) + (1 - Math.Cos(angle)) * x * z;
			m_rotMatrix[1, 2] = x * Math.Sin(angle) + (1 - Math.Cos(angle)) * y * z;
			m_rotMatrix[2, 2] = 1 + (1 - Math.Cos(angle)) * (z * z - 1);
		}

		readonly BasisType m_basisType;
		readonly InterpolationType m_interpolationType;
		readonly uint m_seed;

		double[] m_scale;
		double[] m_offset;
		double m_cos2d;
		double m_sin2d;
		double[,] m_rotMatrix;
		InterpolationFunc m_interpolationFunc;
		NoiseFunc m_noiseFunc;
	}
}
