using System;
using System.Windows;
using System.Windows.Data;

namespace GoldenAnvil.Utility.Views
{
	public sealed class TrimmedElementToVisibilityConverter : IValueConverter
	{
		public static readonly TrimmedElementToVisibilityConverter Instance = new TrimmedElementToVisibilityConverter();

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is FrameworkElement element))
				return Visibility.Collapsed;

			element.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));

			return element.ActualWidth < element.DesiredSize.Width ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
