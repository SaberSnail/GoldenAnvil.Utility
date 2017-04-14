using System;
using System.Windows.Markup;

namespace GoldenAnvil.Utility
{
	public static class RandomUtility
	{
		public static int NextRoll(this Random rng, int number, int dice)
		{
			if (number <= 0)
				throw new ArgumentOutOfRangeException("number", "Number must be greater than 0.");
			if (dice <= 0)
				throw new ArgumentOutOfRangeException("dice", "Dice must be greater than 0.");

			int value = 0;
			for (int i = 0; i < number; i++)
				value += rng.Next(1, dice + 1);
			return value;
		}

		public static double NextDouble(this Random rng, double low, double high)
		{
			return low + rng.NextDouble() * (high - low);
		}

		public static long NextLong(this Random rng)
		{
			byte[] buffer = new byte[8];
			rng.NextBytes(buffer);
			return BitConverter.ToInt64(buffer, 0);
		}

		/// <summary>
		/// Uses the cumulative distribution function of an exponential density function.
		/// Mathematical Statistics and Data Analysis, Second Edition, John A. Rice, 2.3 Example E, pg 61
		/// </summary>
		public static double NextWaitTime(this Random rng, double expectedWaitTime)
		{
			return -expectedWaitTime * Math.Log(rng.NextDouble());
		}

		public static double ProbabilityOfZeroOccurrences(TimeSpan meanTimeToOccurrence, TimeSpan period)
		{
			return Math.Exp(-(double) period.Ticks / (double) meanTimeToOccurrence.Ticks);
		}

		/// <summary>
		/// Uses the Marsaglia polar method.
		/// http://en.wikipedia.org/wiki/Marsaglia_polar_method
		/// </summary>
		public static double NextGauss(this Random rng)
		{
			double a;
			double b;
			double r;
			double f;

			if (m_reuseGauss)
			{
				m_reuseGauss = false;
				return m_gaussG;
			}

			do
			{
				a = 2.0 * rng.NextDouble() - 1.0;
				b = 2.0 * rng.NextDouble() - 1.0;
				r = a * a + b * b;
			} while (r > 1.0 || r == 0.0);
			f = Math.Sqrt(-2.0 * Math.Log(r) / r);
			m_gaussG = a * f;
			m_reuseGauss = true;
			return b * f;
		}

		private static bool m_reuseGauss = false;
		private static double m_gaussG = 0;

		public static double GetPerlinNoise(double x, double y, double z)
		{
			// find relative x,y,z of point in cube
			x -= Math.Floor(x);
			y -= Math.Floor(y);
			z -= Math.Floor(z);

			// compute fade curves for each of x,y,z
			double u = fade(x);
			double v = fade(y);
			double w = fade(z);

			// find unit cube that contains point
			int uX = (int) Math.Floor(x) & 255;
			int uY = (int) Math.Floor(y) & 255;
			int uZ = (int) Math.Floor(z) & 255;

			// hash coordinates of the 8 cube corners
			int hA = p[uX] + uY;
			int hAA = p[hA] + uZ;
			int hAB = p[hA + 1] + uZ;
			int hB = p[uX + 1] + uY;
			int hBA = p[hB] + uZ;
			int hBB = p[hB + 1] + uZ;

			// and add blended results from 8 corners of cube
			return lerp(
				w,
				lerp(v, lerp(u, grad(p[hAA], x, y, z), grad(p[hBA], x - 1, y, z)), lerp(u, grad(p[hAB], x, y - 1, z), grad(p[hBB], x - 1, y - 1, z))),
				lerp(v, lerp(u, grad(p[hAA + 1], x, y, z - 1), grad(p[hBA + 1], x - 1, y, z - 1)),
					lerp(u, grad(p[hAB + 1], x, y - 1, z - 1), grad(p[hBB + 1], x - 1, y - 1, z - 1))));
		}

		static RandomUtility()
		{
			for (int i = 0; i < 256; i++)
				p[256 + i] = p[i] = permutation[i];
		}

		private static double fade(double t)
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		private static double lerp(double t, double a, double b)
		{
			return a + t * (b - a);
		}

		private static double grad(int hash, double x, double y, double z)
		{
			// convert low 4 bits of hash code into 12 gradient directions
			int h = hash & 15;
			double u = h < 8 ? x : y;
			double v = h < 4 ? y : h == 12 || h == 14 ? x : z;
			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}

		private static readonly int[] p = new int[512], permutation = { 151, 160, 137, 91, 90, 15,
				131, 13, 201, 95, 96, 53, 194, 233, 7, 225, 140, 36, 103, 30, 69, 142, 8, 99, 37, 240, 21, 10, 23,
				190, 6, 148, 247, 120, 234, 75, 0, 26, 197, 62, 94, 252, 219, 203, 117, 35, 11, 32, 57, 177, 33,
				88, 237, 149, 56, 87, 174, 20, 125, 136, 171, 168, 68, 175, 74, 165, 71, 134, 139, 48, 27, 166,
				77, 146, 158, 231, 83, 111, 229, 122, 60, 211, 133, 230, 220, 105, 92, 41, 55, 46, 245, 40, 244,
				102, 143, 54, 65, 25, 63, 161, 1, 216, 80, 73, 209, 76, 132, 187, 208, 89, 18, 169, 200, 196,
				135, 130, 116, 188, 159, 86, 164, 100, 109, 198, 173, 186, 3, 64, 52, 217, 226, 250, 124, 123,
				5, 202, 38, 147, 118, 126, 255, 82, 85, 212, 207, 206, 59, 227, 47, 16, 58, 17, 182, 189, 28, 42,
				223, 183, 170, 213, 119, 248, 152, 2, 44, 154, 163, 70, 221, 153, 101, 155, 167, 43, 172, 9,
				129, 22, 39, 253, 19, 98, 108, 110, 79, 113, 224, 232, 178, 185, 112, 104, 218, 246, 97, 228,
				251, 34, 242, 193, 238, 210, 144, 12, 191, 179, 162, 241, 81, 51, 145, 235, 249, 14, 239, 107,
				49, 192, 214, 31, 181, 199, 106, 157, 184, 84, 204, 176, 115, 121, 50, 45, 127, 4, 150, 254,
				138, 236, 205, 93, 222, 114, 67, 29, 24, 72, 243, 141, 128, 195, 78, 66, 215, 61, 156, 180
		};
	}
}
