/*
This code is adapted from the AccidentalNoise library at http://accidentalnoise.sourceforge.net

Copyright (c) 2008 Joshua Tippetts

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using InterpolationFunc = System.Func<double, double>;
using WorkerNoise2D = System.Func<double, double, int, int, uint, double>;

namespace AccidentalNoise
{
	internal static class NoiseUtility
	{
		public static double Lerp(double t, double a, double b)
		{
			return a + t * (b - a);
		}

		public static int FastFloor(double t)
		{
			return t > 0 ? (int) t : (int) (t - 1);
		}

		public static double Bias(double b, double t)
		{
			return Math.Pow(t, Math.Log(b) / Math.Log(0.5));
		}

		public static double DotProduct2D(double[] array, double a, double b)
		{
			return a * array[0] + b * array[1];
		}

		public static double NoInterpolation(double t)
		{
			return 0;
		}

		public static double LinearInterpolation(double t)
		{
			return t;
		}

		public static double HermiteInterpolation(double t)
		{
			return (t * t * (3 - 2 * t));
		}

		public static double QuinticInterpolation(double t)
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		public static double GetValueNoise2D(double x, double y, uint seed, InterpolationFunc interpolationFunc)
		{
			int x0 = FastFloor(x);
			int y0 = FastFloor(y);

			int x1 = x0 + 1;
			int y1 = y0 + 1;

			double xs = interpolationFunc((x - (double) x0));
			double ys = interpolationFunc((y - (double) y0));

			return InterpolateXY2D(x, y, xs, ys, x0, x1, y0, y1, seed, GetValueNoise2D);
		}

		private static double GetValueNoise2D(double x, double y, int ix, int iy, uint seed)
		{
			uint n = HashCoordinates2D(ix, iy, seed);
			double noise = (double) n / 255.0;
			return noise * 2.0 - 1.0;
		}

		public static double GetGradientNoise2D(double x, double y, uint seed, InterpolationFunc interpolationFunc)
		{
			int x0 = FastFloor(x);
			int y0 = FastFloor(y);

			int x1 = x0 + 1;
			int y1 = y0 + 1;

			double xs = interpolationFunc((x - (double) x0));
			double ys = interpolationFunc((y - (double) y0));

			return InterpolateXY2D(x, y, xs, ys, x0, x1, y0, y1, seed, GetGradientNoise2D);
		}

		private static double GetGradientNoise2D(double x, double y, int ix, int iy, uint seed)
		{
			uint hash = HashCoordinates2D(ix, iy, seed);

			double dx = x - (double) ix;
			double dy = y - (double) iy;

			return (dx * s_gradient2DLut[hash,0] + dy * s_gradient2DLut[hash,0]);
		}

		public static double GetGradientValueNoise2D(double x, double y, uint seed, InterpolationFunc interpolationFunc)
		{
			return GetValueNoise2D(x, y, seed, interpolationFunc) + GetGradientNoise2D(x, y, seed, interpolationFunc);
		}

		public static double GetWhiteNoise2D(double x, double y, uint seed, InterpolationFunc interpolationFunc)
		{
			byte hash = (byte) HashCoordinates2D(x, y, seed);
			return s_whitenoiseLut[hash];
		}

		public static double GetSimplexNoise2D(double x, double y, uint seed, InterpolationFunc interpolationFunc)
		{
			double s = (x + y) * c_f2;
			int i = FastFloor(x + s);
			int j = FastFloor(y + s);

			double t = (i + j) * c_g2;
			double X0 = i - t;
			double Y0 = j - t;
			double x0 = x - X0;
			double y0 = y - Y0;

			int i1, j1;
			if (x0 > y0)
			{
				i1 = 1;
				j1 = 0;
			}
			else
			{
				i1 = 0;
				j1 = 1;
			}

			double x1 = x0 - (double) i1 + c_g2;
			double y1 = y0 - (double) j1 + c_g2;
			double x2 = x0 - 1.0 + 2.0 * c_g2;
			double y2 = y0 - 1.0 + 2.0 * c_g2;

			// Hash the triangle coordinates to index the gradient table
			uint h0 = HashCoordinates2D(i, j, seed);
			uint h1 = HashCoordinates2D(i + i1, j + j1, seed);
			uint h2 = HashCoordinates2D(i + 1, j + 1, seed);

			// Now, index the tables
			double[] g0 = { s_gradient2DLut[h0, 0], s_gradient2DLut[h0, 1] };
			double[] g1 = { s_gradient2DLut[h1, 0], s_gradient2DLut[h1, 1] };
			double[] g2 = { s_gradient2DLut[h2, 0], s_gradient2DLut[h2, 1] };

			double n0, n1, n2;
			// Calculate the contributions from the 3 corners
			double t0 = 0.5 - x0 * x0 - y0 * y0;
			if (t0 < 0) n0 = 0;
			else
			{
				t0 *= t0;
				n0 = t0 * t0 * DotProduct2D(g0, x0, y0);
			}

			double t1 = 0.5 - x1 * x1 - y1 * y1;
			if (t1 < 0) n1 = 0;
			else
			{
				t1 *= t1;
				n1 = t1 * t1 * DotProduct2D(g1, x1, y1);
			}

			double t2 = 0.5 - x2 * x2 - y2 * y2;
			if (t2 < 0) n2 = 0;
			else
			{
				t2 *= t2;
				n2 = t2 * t2 * DotProduct2D(g2, x2, y2);
			}

			// Add contributions together
			return (70.0 * (n0 + n1 + n2)) * 1.42188695 + 0.001054489;
		}

		private static uint HashCoordinates2D(int x, int y, uint seed)
		{
			uint[] values = { (uint) x, (uint) y, seed };
			return XorFoldHash(Fnv32ABuf(ConvertToBytes(values)));
		}

		private static uint HashCoordinates2D(double x, double y, uint seed)
		{
			double[] values = { x, y, (double) seed };
			return XorFoldHash(Fnv32ABuf(ConvertToBytes(values)));
		}

		private static IEnumerable<byte> ConvertToBytes(uint[] values)
		{
			byte[] bytes = new byte[values.Length * 4];
			for (int i = 0, j = 0; i < values.Length; i++)
			{
				bytes[j++] = (byte) (values[i] & 0xFF);
				bytes[j++] = (byte) ((values[i] >> 8) & 0xFF);
				bytes[j++] = (byte) ((values[i] >> 16) & 0xFF);
				bytes[j++] = (byte) ((values[i] >> 24) & 0xFF);
			}
			return bytes;
		}

		private static IEnumerable<byte> ConvertToBytes(double[] values)
		{
			byte[] bytes = new byte[values.Length * 8];
			int r = 0;

			for (int i = 0; i < values.Length; i++)
			{
				byte[] value = System.BitConverter.GetBytes(values[i]);
				for (int j = 0; j < value.Length; j++)
				{
					bytes[r] = value[j];
					r++;
				}
			}

			return bytes;
		}

		private static byte XorFoldHash(uint hash)
		{
			return (byte) ((hash >> 8) ^ (hash & c_fnvMask8));
		}

		private static uint Fnv32ABuf(IEnumerable<byte> bytes)
		{
			uint hval = c_fnv32Init;
			foreach (byte b in bytes)
			{
				hval ^= b;
				hval *= c_fnv32Prime;
			}
			return hval;
		}

		private static double InterpolateX2D(double x, double y, double xs, int x0, int x1, int iy, uint seed, WorkerNoise2D noiseFunc)
		{
			double v1 = noiseFunc(x, y, x0, iy, seed);
			double v2 = noiseFunc(x, y, x1, iy, seed);
			return Lerp(xs, v1, v2);
		}

		private static double InterpolateXY2D(double x, double y, double xs, double ys, int x0, int x1, int y0, int y1, uint seed, WorkerNoise2D noiseFunc)
		{
			double v1 = InterpolateX2D(x, y, xs, x0, x1, y0, seed, noiseFunc);
			double v2 = InterpolateX2D(x, y, xs, x0, x1, y1, seed, noiseFunc);
			return Lerp(ys, v1, v2);
		}

		const uint c_fnvMask8 = ((uint) 1 << 8) - 1;
		const uint c_fnv32Init = 2166136261;
		const uint c_fnv32Prime = 0x01000193;
		const double c_f2 = 0.36602540378443864676372317075294;
		const double c_g2 = 0.21132486540518711774542560974902;

		#region Lookup Tables

		// Generated with boost::random, using a lagged Fibonacci generator and a uniform_on_sphere distribution.
		static readonly double[,] s_gradient2DLut =
		{
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0},
			{0,1}, {0,-1}, {1,0}, {-1,0}, {0,1}, {0,-1}, {1,0}, {-1,0}
		};

		static readonly double[] s_whitenoiseLut =
		{
			-0.714286, 0.301587, 0.333333, -1, 0.396825, -0.0793651, -0.968254, -0.047619,
			0.301587, -0.111111, 0.015873, 0.968254, -0.428571, 0.428571, 0.047619, 0.84127,
			-0.015873, -0.746032, -0.809524, -0.619048, -0.301587, -0.68254, 0.777778, 0.365079,
			-0.460317, 0.714286, 0.142857, 0.047619, -0.0793651, -0.492063, -0.873016, -0.269841,
			-0.84127, -0.809524, -0.396825, -0.777778, -0.396825, -0.746032, 0.301587, -0.52381,
			0.650794, 0.301587, -0.015873, 0.269841, 0.492063, -0.936508, -0.777778, 0.555556,
			0.68254, -0.650794, -0.968254, 0.619048, 0.777778, 0.68254, 0.206349, -0.555556,
			0.904762, 0.587302, -0.174603, -0.047619, -0.206349, -0.68254, 0.111111, -0.52381,
			0.174603, -0.968254, -0.111111, -0.238095, 0.396825, -0.777778, -0.206349, 0.142857,
			0.904762, -0.111111, -0.269841, 0.777778, -0.015873, -0.047619, -0.333333, 0.68254,
			-0.238095, 0.904762, 0.0793651, 0.68254, -0.301587, -0.333333, 0.206349, 0.52381,
			0.904762, -0.015873, -0.555556, 0.396825, 0.460317, -0.142857, 0.587302, 1,
			-0.650794, -0.333333, -0.365079, 0.015873, -0.873016, -1, -0.777778, 0.174603,
			-0.84127, -0.428571, 0.365079, -0.587302, -0.587302, 0.650794, 0.714286, 0.84127,
			0.936508, 0.746032, 0.047619, -0.52381, -0.714286, -0.746032, -0.206349, -0.301587,
			-0.174603, 0.460317, 0.238095, 0.968254, 0.555556, -0.269841, 0.206349, -0.0793651,
			0.777778, 0.174603, 0.111111, -0.714286, -0.84127, -0.68254, 0.587302, 0.746032,
			-0.68254, 0.587302, 0.365079, 0.492063, -0.809524, 0.809524, -0.873016, -0.142857,
			-0.142857, -0.619048, -0.873016, -0.587302, 0.0793651, -0.269841, -0.460317, -0.904762,
			-0.174603, 0.619048, 0.936508, 0.650794, 0.238095, 0.111111, 0.873016, 0.0793651,
			0.460317, -0.746032, -0.460317, 0.428571, -0.714286, -0.365079, -0.428571, 0.206349,
			0.746032, -0.492063, 0.269841, 0.269841, -0.365079, 0.492063, 0.873016, 0.142857,
			0.714286, -0.936508, 1, -0.142857, -0.904762, -0.301587, -0.968254, 0.619048,
			0.269841, -0.809524, 0.936508, 0.714286, 0.333333, 0.428571, 0.0793651, -0.650794,
			0.968254, 0.809524, 0.492063, 0.555556, -0.396825, -1, -0.492063, -0.936508,
			-0.492063, -0.111111, 0.809524, 0.333333, 0.238095, 0.174603, 0.333333, 0.873016,
			0.809524, -0.047619, -0.619048, -0.174603, 0.84127, 0.111111, 0.619048, -0.0793651,
			0.52381, 1, 0.015873, 0.52381, -0.619048, -0.52381, 1, 0.650794,
			-0.428571, 0.84127, -0.555556, 0.015873, 0.428571, 0.746032, -0.238095, -0.238095,
			0.936508, -0.206349, -0.936508, 0.873016, -0.555556, -0.650794, -0.904762, 0.52381,
			0.968254, -0.333333, -0.904762, 0.396825, 0.047619, -0.84127, -0.365079, -0.587302,
			-1, -0.396825, 0.365079, 0.555556, 0.460317, 0.142857, -0.460317, 0.238095,
		};

		#endregion
	}
}
