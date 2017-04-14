using System;
using System.Windows.Media;

namespace GoldenAnvil.Utility.Views
{
	public static class ColorUtility
	{
		/// <summary>
		/// Create a Color from hue, saturation and value.
		/// </summary>
		/// <param name="hue">0-360 hue</param>
		/// <param name="saturation">0-1 saturation</param>
		/// <param name="value">0-1 value</param>
		/// <returns>A Color</returns>
		public static Color FromHSV(double hue, double saturation, double value)
		{
			return FromAHSV(255, hue, saturation, value);
		}

		/// <summary>
		/// Create a Color from alpha, hue, saturation and value.
		/// </summary>
		/// <param name="alpha">0-255 alpha (0 is transparent)</param>
		/// <param name="hue">0-360 hue</param>
		/// <param name="saturation">0-1 saturation</param>
		/// <param name="value">0-1 value</param>
		/// <returns>A Color</returns>
		public static Color FromAHSV(byte alpha, double hue, double saturation, double value)
		{
			double red;
			double green;
			double blue;

			hue = hue % 360;
			if (hue < 0)
				hue += 360;

			if (value <= 0)
			{
				red = 0;
				green = 0;
				blue = 0;
			}
			else if (saturation <= 0)
			{
				red = value;
				green = value;
				blue = value;
			}
			else
			{
				double hf = hue / 60.0;
				int i = (int) Math.Floor(hf);
				double f = hf - i;
				double pv = value * (1 - saturation);
				double qv = value * (1 - saturation * f);
				double tv = value * (1 - saturation * (1 - f));
				switch (i)
				{
				// Red is the dominant color
				case 0:
					red = value;
					green = tv;
					blue = pv;
					break;

				// Green is the dominant color
				case 1:
					red = qv;
					green = value;
					blue = pv;
					break;
				case 2:
					red = pv;
					green = value;
					blue = tv;
					break;

				// Blue is the dominant color
				case 3:
					red = pv;
					green = qv;
					blue = value;
					break;
				case 4:
					red = tv;
					green = pv;
					blue = value;
					break;

				// Red is the dominant color
				case 5:
					red = value;
					green = pv;
					blue = qv;
					break;

				// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
				case 6:
					red = value;
					green = tv;
					blue = pv;
					break;
				case -1:
					red = value;
					green = pv;
					blue = qv;
					break;

				// The color is not defined, just pretend it's black/white
				default:
					red = value;
					green = value;
					blue = value;
					break;
				}
			}

			red = MathUtility.Clamp(red * 255, 0, 255);
			green = MathUtility.Clamp(green * 255, 0, 255);
			blue = MathUtility.Clamp(blue * 255, 0, 255);

			return Color.FromArgb(alpha, (byte) red, (byte) green, (byte) blue);
		}
	}
}
