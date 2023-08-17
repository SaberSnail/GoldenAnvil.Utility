using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace GoldenAnvil.Utility.Windows.Behaviors
{
	public sealed class FocusAfterClickBehavior : Behavior<ButtonBase>
	{
		public static readonly DependencyProperty FocusElementProperty = DependencyPropertyUtility<FocusAfterClickBehavior>.Register(x => x.FocusElement);

		public UIElement FocusElement
		{
			get => (UIElement) GetValue(FocusElementProperty);
			set => SetValue(FocusElementProperty, value);
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.Click += OnClick;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.Click -= OnClick;
		}

		private void OnClick(object sender, RoutedEventArgs e)
		{
			if (FocusElement is not null)
			{
				foreach (var element in VisualTreeUtility.EnumerateDescendants(FocusElement).OfType<UIElement>())
				{
					if (element.Focusable)
					{
						element?.Focus();
						break;
					}
				}
			}
		}
	}
}
