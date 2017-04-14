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

namespace AccidentalNoise
{
	public abstract class RandomNumberGeneratorBase
	{
		public abstract uint Next();

		public uint NextTarget(uint t)
		{
			double v = Next01();
			return (uint) (v * (double) t);
		}

		public uint NextRange(uint low, uint high)
		{
			if (high < low)
				return NextRange(high, low);

			double range = (double) (high - low + 1);
			double value = (double) low + Next01() * range;
			return (uint) value;
		}

		public double Next01()
		{
			return ((double) Next() / (double) 4294967295UL);
		}
	}

	public sealed class RandomLCG : RandomNumberGeneratorBase
	{
		public RandomLCG(uint seed)
		{
			SetSeed(seed);
		}

		public override uint Next()
		{
			m_state = 69069 * m_state + 362437;
			return m_state;
		}

		private void SetSeed(uint seed)
		{
			m_state = seed;
		}

		uint m_state;
	}

	// The following generator is based on a generator created by George Marsaglia
	public sealed class RandomXorShift : RandomNumberGeneratorBase
	{
		public RandomXorShift(uint seed)
		{
			SetSeed(seed);
		}

		public override uint Next()
		{
			uint t = (m_x ^ (m_x >> 7));
			m_x = m_y;
			m_y = m_z;
			m_z = m_w;
			m_w = m_v;
			m_v = (m_v ^ (m_v << 6)) ^ (t ^ (t << 13));
			return (m_y + m_y + 1) * m_v;
		}

		private void SetSeed(uint seed)
		{
			RandomLCG lcg = new RandomLCG(seed);
			m_x = lcg.Next();
			m_y = lcg.Next();
			m_z = lcg.Next();
			m_w = lcg.Next();
			m_v = lcg.Next();
		}

		uint m_x;
		uint m_y;
		uint m_z;
		uint m_w;
		uint m_v;
	}

	// The following generator is based on a generator created by George Marsaglia
	public sealed class RandomMWC256 : RandomNumberGeneratorBase
	{
		public RandomMWC256(uint seed)
		{
			m_i = 255;
			m_q = new uint[256];
			SetSeed(seed);
		}

		public override uint Next()
		{
			const ulong a = 809430660UL;
			ulong t = a * m_q[++m_i] + m_c;
			m_c = (uint) (t >> 32);
			return (m_q[m_i] = (uint) t);
		}

		private void SetSeed(uint seed)
		{
			RandomLCG lcg = new RandomLCG(seed);
			for (int i = 0; i < 256; i++)
				m_q[i] = lcg.Next();
			m_c = lcg.NextTarget(809430660);
		}

		readonly uint[] m_q;
		uint m_c;
		byte m_i;
	}

	// The following generator is based on a generator created by George Marsaglia
	public sealed class RandomCMWC4096 : RandomNumberGeneratorBase
	{
		public RandomCMWC4096(uint seed)
		{
			m_i = 2095;
			m_q = new uint[4096];
			SetSeed(seed);
		}

		public override uint Next()
		{
			const ulong a = 18782UL;
			const ulong b = 4294967295UL;
			const uint r = (uint) (b - 1);

			m_i = (m_i + 1) & 4095;
			ulong t = a * m_q[m_i] + m_c;
			m_c = (uint) (t >> 32);
			t = (t & b) + m_c;
			if (t > r)
			{
				m_c++;
				t = t - b;
			}
			return (m_q[m_i] = (uint) (r - t));
		}

		private void SetSeed(uint seed)
		{
			RandomLCG lcg = new RandomLCG(seed);
			for (int i = 0; i < 256; i++)
				m_q[i] = lcg.Next();
			m_c = lcg.NextTarget(18781);
		}

		readonly uint[] m_q;
		uint m_c;
		uint m_i;
	}
}
