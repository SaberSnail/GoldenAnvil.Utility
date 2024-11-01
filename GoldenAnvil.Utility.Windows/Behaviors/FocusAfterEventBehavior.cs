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

			AssociatedObject.DataContextChanged += AssociatedObject_DataContextChanged;

			var viewModel = AssociatedObject.DataContext;
			if (viewModel is not null)
				AddEventHandler(viewModel);
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			AssociatedObject.DataContextChanged -= AssociatedObject_DataContextChanged;

			var viewModel = AssociatedObject.DataContext;
			if (viewModel is not null)
				RemoveEventHandler(viewModel);
		}

		private void AddEventHandler(object viewModel)
		{
			var eventInfo = viewModel.GetType().GetEvent(EventName);
			var methodInfo = GetType().GetMethod(nameof(OnEvent), BindingFlags.NonPublic | BindingFlags.Instance);
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
			eventInfo.AddEventHandler(viewModel, handler);
		}

		private void RemoveEventHandler(object viewModel)
		{
			var eventInfo = viewModel.GetType().GetEvent(EventName);
			var methodInfo = GetType().GetMethod(nameof(OnEvent), BindingFlags.NonPublic | BindingFlags.Instance);
			var handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, methodInfo);
			eventInfo.RemoveEventHandler(viewModel, handler);
		}

		private void AssociatedObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (e.OldValue is not null)
				RemoveEventHandler(e.OldValue);
			if (e.NewValue is not null)
				AddEventHandler(e.NewValue);
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
