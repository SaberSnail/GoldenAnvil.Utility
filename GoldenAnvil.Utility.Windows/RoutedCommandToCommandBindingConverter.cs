using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace GoldenAnvil.Utility.Windows
{
	public sealed class RoutedCommandToCommandBindingConverter : IMultiValueConverter
	{
		public static readonly RoutedCommandToCommandBindingConverter Instance = new RoutedCommandToCommandBindingConverter();

		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			List<CommandBinding> commandBindings = new List<CommandBinding>();
			for (var index = 0; index < values.Length; index++)
			{
				var routedCommand = values[index] as RoutedCommand;
				index++;
				if (index != values.Length)
				{
					var command = values[index] as ICommand;
					if (routedCommand != null && command != null)
					{
						commandBindings.Add(new CommandBinding(
							routedCommand,
							(sender, args) => command.Execute(args.Parameter),
							(sender, args) => args.CanExecute = command.CanExecute(args.Parameter)));
					}
				}
			}

			return commandBindings;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
