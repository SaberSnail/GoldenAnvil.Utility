using System;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace GoldenAnvil.Utility.Windows
{
	public sealed class MethodToCommandConverter : IValueConverter
	{
		public static readonly MethodToCommandConverter Instance = new MethodToCommandConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return DependencyProperty.UnsetValue;
			if (!(typeof(ICommand)).IsAssignableFrom(targetType))
				throw new ArgumentException($"Converter may only be used if targetType ({targetType.Name}) is assignable to ICommand", nameof(targetType));
			if (!(parameter is string))
				throw new ArgumentException("Parameter must be a string indicating the method name", nameof(parameter));

			Type valueType = value.GetType();
			MethodInfo method = valueType.GetMethod((string) parameter, BindingFlags.Instance | BindingFlags.Public);
			if (method == null)
				throw new ArgumentException($"Method ({parameter}) not found on type ({valueType.Name})");

			ICommand command;
			ParameterInfo[] methodParameters = method.GetParameters();
			if (methodParameters.Length == 0)
			{
				Action execute = (Action) Delegate.CreateDelegate(typeof(Action), value, method);

				MethodInfo canMethod = valueType.GetMethod("Can" + (string) parameter, BindingFlags.Instance | BindingFlags.Public);
				if (canMethod != null && canMethod.GetParameters().Length == 0)
				{
					Func<bool> canExecute = (Func<bool>) Delegate.CreateDelegate(typeof(Func<bool>), value, canMethod);
					command = new DelegateCommand(execute, canExecute);
				}
				else
				{
					command = new DelegateCommand(execute);
				}
			}
			else if (methodParameters.Length == 1)
			{
				Type parameterType = methodParameters[0].ParameterType;
				MethodInfo createAction = typeof(MethodToCommandConverter).GetMethod("CreateCommand", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(parameterType);

				MethodInfo canMethod = valueType.GetMethod("Can" + (string) parameter, BindingFlags.Instance | BindingFlags.Public);
				if (canMethod != null && canMethod.GetParameters().Length == 1)
				{
					command = (ICommand) createAction.Invoke(null, new[] { value, method, canMethod });
				}
				else
				{
					command = (ICommand) createAction.Invoke(null, new[] { value, method });
				}
			}
			else
			{
				throw new ArgumentException($"Method ({parameter}) found on type ({valueType.Name}) may only have 0 or 1 parameter");
			}

			return command;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DependencyProperty.UnsetValue;
		}

		private static ICommand CreateCommand<T>(object value, MethodInfo method)
		{
			Action<T> execute = (Action<T>) Delegate.CreateDelegate(typeof(Action<T>), value, method);
			return new DelegateCommand<T>(execute);
		}

		private static ICommand CreateCommand<T>(object value, MethodInfo method, MethodInfo canMethod)
		{
			Action<T> execute = (Action<T>) Delegate.CreateDelegate(typeof(Action<T>), value, method);
			Predicate<T> canExecute = (Predicate<T>) Delegate.CreateDelegate(typeof(Predicate<T>), value, canMethod);
			return new DelegateCommand<T>(execute, canExecute);
		}
	}
}
