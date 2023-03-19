using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoldenAnvil.Utility.Windows
{
	public static class TreeViewUtility
	{
		public static readonly DependencyProperty SelectedItemChangedCommandProperty =
			DependencyProperty.RegisterAttached("SelectedItemChangedCommand", typeof(ICommand), typeof(TreeViewUtility), new PropertyMetadata(new PropertyChangedCallback(AttachOrRemoveSelectedItemChangedEvent)));

		public static ICommand GetSelectedItemChangedCommand(DependencyObject obj)
		{
			return (ICommand) obj.GetValue(SelectedItemChangedCommandProperty);
		}

		public static void SetSelectedItemChangedCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(SelectedItemChangedCommandProperty, value);
		}

		public static void AttachOrRemoveSelectedItemChangedEvent(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			if (obj is TreeView treeView)
			{
				if (args.OldValue is null && args.NewValue is not null)
					treeView.SelectedItemChanged += OnSelectedItemChanged;
				else if (args.OldValue is not null && args.NewValue is null)
					treeView.SelectedItemChanged -= OnSelectedItemChanged;
			}
		}

		private static void OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
		{
			var obj = sender as DependencyObject;
			var command = (ICommand) obj.GetValue(SelectedItemChangedCommandProperty);
			if (command != null)
			{
				if (command.CanExecute(args.NewValue))
					command.Execute(args.NewValue);
			}
		}
	}
}
