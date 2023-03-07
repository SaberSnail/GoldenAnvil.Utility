using System;
using System.Globalization;
using System.Linq;
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

		public static readonly IValueConverter IsNullToVisibility = new IsNullToVisibilityConverter();

		public static readonly IMultiValueConverter Max = new MaxConverter();

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

		private sealed class IsNullToVisibilityConverter : IValueConverter
		{
			public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
			{
				if (!targetType.IsAssignableFrom(typeof(Visibility)))
					throw new InvalidOperationException(@"The target must be assignable from a Visibility.");

				return value is null ? Visibility.Collapsed : Visibility.Visible;
			}

			public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}

		private sealed class MaxConverter : IMultiValueConverter
		{
			object IMultiValueConverter.Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
			{
				return values.Cast<IComparable>().Max();
			}

			object[] IMultiValueConverter.ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
			{
				throw new NotImplementedException();
			}
		}
	}
}
