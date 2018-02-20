using System;
using System.Resources;
using System.Windows.Data;

namespace GoldenAnvil.Utility.Windows
{
	public sealed class EnumToDisplayStringConverter : IValueConverter
	{
		public static readonly EnumToDisplayStringConverter Instance = new EnumToDisplayStringConverter();

		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(parameter is ResourceManager resources))
				throw new ArgumentException($"{nameof(parameter)} must be a ResourceManager.");

			var method = typeof(ResourceManagerUtility).GetMethod("EnumToDisplayString");
			var generic = method.MakeGenericMethod(value.GetType());
			return generic.Invoke(null, new [] { resources, value });
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
