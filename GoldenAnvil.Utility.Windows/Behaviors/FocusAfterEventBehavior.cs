using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace GoldenAnvil.Utility.Windows.Behaviors
{
	public sealed class FocusAfterEventBehavior : Behavior<FrameworkElement>
	{
		public static readonly DependencyProperty EventProperty = DependencyPropertyUtility<FocusAfterEventBehavior>.Register(x => x.EventName);

		public string EventName
		{
			get => (string) GetValue(EventProperty);
			set => SetValue(EventProperty, value);
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			var viewModel = AssociatedObject.DataContext;
			var eventInfo = viewModel.GetType().GetEvent(EventName);
			var methodInfo = GetType().GetMethod(nameof(OnEvent), BindingFlags.NonPublic | BindingFlags.Instance);
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
			eventInfo.AddEventHandler(viewModel, handler);
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			var viewModel = AssociatedObject.DataContext;
			var eventInfo = viewModel.GetType().GetEvent(EventName);
			var methodInfo = GetType().GetMethod(nameof(OnEvent), BindingFlags.NonPublic | BindingFlags.Instance);
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
			eventInfo.RemoveEventHandler(viewModel, handler);
		}

		private void OnEvent(object sender, EventArgs e)
		{
			foreach (var element in VisualTreeUtility.EnumerateDescendants(AssociatedObject).OfType<UIElement>())
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
