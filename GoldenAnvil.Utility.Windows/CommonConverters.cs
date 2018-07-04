using System;
using System.Globalization;
using System.Windows.Data;

namespace GoldenAnvil.Utility.Windows
{
	public static class CommonConverters
	{
		public static readonly IValueConverter BooleanNot = new BooleanNotConverter();

		private sealed class BooleanNotConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (targetType != typeof(bool))
					throw new InvalidOperationException(@"The target must be a boolean.");

				return !((bool) value);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
	}
}
