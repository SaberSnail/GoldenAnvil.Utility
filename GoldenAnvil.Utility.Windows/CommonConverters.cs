using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GoldenAnvil.Utility.Windows
{
	public static class CommonConverters
	{
		public static readonly IValueConverter BooleanNot = new BooleanNotConverter();

		public static readonly IValueConverter BooleanToVisibility = new BooleanToVisibilityConverter();

		public static readonly IValueConverter IsEqual = new IsEqualConverter();

		public static readonly IValueConverter IsEqualToVisibility = new IsEqualToVisibilityConverter();

		private sealed class BooleanNotConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (!targetType.IsAssignableFrom(typeof(bool)))
					throw new InvalidOperationException(@"The target must be assignable from a boolean.");

				return !((bool) value);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		private sealed class BooleanToVisibilityConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (!targetType.IsAssignableFrom(typeof(Visibility)))
					throw new InvalidOperationException(@"The target must be assignable from a Visibility.");

				return (bool) value ? Visibility.Visible : Visibility.Collapsed;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		private sealed class IsEqualConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (!targetType.IsAssignableFrom(typeof(bool)))
					throw new InvalidOperationException(@"The target must be assignable from a boolean.");

				return object.Equals(value, parameter);
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		private sealed class IsEqualToVisibilityConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (!targetType.IsAssignableFrom(typeof(Visibility)))
					throw new InvalidOperationException(@"The target must be assignable from a Visibility.");

				return object.Equals(value, parameter) ? Visibility.Visible : Visibility.Collapsed;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
	}
}
