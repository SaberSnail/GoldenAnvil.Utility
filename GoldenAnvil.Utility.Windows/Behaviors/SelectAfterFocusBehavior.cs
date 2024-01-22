using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace GoldenAnvil.Utility.Windows.Behaviors
{
	public sealed class SelectAfterFocusBehavior : Behavior<TextBox>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.GotFocus += OnFocus;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.GotFocus -= OnFocus;
		}

		private void OnFocus(object sender, RoutedEventArgs e)
		{
			AssociatedObject.SelectAll();
		}
	}
}
