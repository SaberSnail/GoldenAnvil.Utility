using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace GoldenAnvil.Utility.Windows
{
	public static class CommandUtility
	{
		public static readonly DependencyProperty MergedCommandBindingsProperty = DependencyProperty.RegisterAttached("MergedCommandBindings", typeof(IReadOnlyCollection<CommandBinding>), typeof(CommandUtility), new PropertyMetadata(null, OnMergedCommandBindingsChanged));

		public static string GetMergedCommandBindings(DependencyObject d)
		{
			return (string) d.GetValue(MergedCommandBindingsProperty);
		}

		public static void SetMergedCommandBindings(DependencyObject d, string value)
		{
			d.SetValue(MergedCommandBindingsProperty, value);
		}

		private static void OnMergedCommandBindingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d == null)
				return;

			var commandBindings = GetCommandBindings(d);
			if (commandBindings is null)
				throw new InvalidOperationException($"{MergedCommandBindingsProperty.Name} is attached to a DependencyObject that does not have CommandBindings.");

			if (e.OldValue != null)
			{
				foreach (var commandBinding in (IReadOnlyCollection<CommandBinding>) e.OldValue)
					commandBindings.Remove(commandBinding);
			}

			if (e.NewValue != null)
			{
				foreach (var commandBinding in (IReadOnlyCollection<CommandBinding>) e.NewValue)
					commandBindings.Add(commandBinding);
			}

			CommandManager.InvalidateRequerySuggested();
		}

		private static CommandBindingCollection GetCommandBindings(DependencyObject d)
		{
			if (d is UIElement uiElement)
				return uiElement.CommandBindings;
			if (d is ContentElement contentElement)
				return contentElement.CommandBindings;
			if (d is UIElement3D uiElement3D)
				return uiElement3D.CommandBindings;

			return null;
		}
	}
}
