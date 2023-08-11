using System;
using System.Linq;
using System.Windows;

namespace GoldenAnvil.Utility.Windows.Themes
{
	public static class ThemesUtility
	{
		public static ResourceDictionary CurrentThemeDictionary { get; private set; }

		public static readonly DependencyProperty CurrentThemeUrlProperty = DependencyProperty.RegisterAttached("CurrentThemeUrl", typeof(Uri), typeof(ThemesUtility), new UIPropertyMetadata(null, OnCurrentThemeDictionaryChanged));

		public static Uri GetCurrentThemeUrl(DependencyObject d)
		{
			return (Uri) d.GetValue(CurrentThemeUrlProperty);
		}

		public static void SetCurrentThemeUrl(DependencyObject d, Uri value)
		{
			d.SetValue(CurrentThemeUrlProperty, value);
		}

		public static readonly DependencyProperty ShouldInvalidateOnThemeChangeProperty = DependencyProperty.RegisterAttached("ShouldInvalidateOnThemeChange", typeof(bool), typeof(ThemesUtility), new UIPropertyMetadata(false, OnShouldInvalidateOnThemeChangeChanged));

		public static bool GetShouldInvalidateOnThemeChange(DependencyObject d)
		{
			return (bool) d.GetValue(ShouldInvalidateOnThemeChangeProperty);
		}

		public static void SetShouldInvalidateOnThemeChange(DependencyObject d, bool value)
		{
			d.SetValue(ShouldInvalidateOnThemeChangeProperty, value);
		}

		public static readonly RoutedEvent NeedsInvalidateVisualEvent = EventManager.RegisterRoutedEvent("NeedsInvalidate", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(ThemesUtility));

		public static void AddNeedsInvalidateVisualHandler(DependencyObject d, RoutedEventHandler handler)
		{
			if (d is UIElement element)
				element.AddHandler(NeedsInvalidateVisualEvent, handler);
		}

		public static void RemoveNeedsInvalidateVisualHandler(DependencyObject d, RoutedEventHandler handler)
		{
			if (d is UIElement element)
				element.RemoveHandler(NeedsInvalidateVisualEvent, handler);
		}

		private static void OnCurrentThemeDictionaryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is FrameworkElement element)
				ApplyTheme(element, GetCurrentThemeUrl(d));
		}

		private static void ApplyTheme(FrameworkElement element, Uri dictionaryUri)
		{
			var existingDictionaries = element.Resources.MergedDictionaries.OfType<ThemeResourceDictionary>().ToList();
			foreach (var dictionary in existingDictionaries)
				element.Resources.MergedDictionaries.Remove(dictionary);

			if (dictionaryUri != null)
			{
				CurrentThemeDictionary = new ThemeResourceDictionary { Source = dictionaryUri };
				element.Resources.MergedDictionaries.Insert(0, CurrentThemeDictionary);
			}
			else
			{
				CurrentThemeDictionary = null;
			}

			element.RaiseEvent(new RoutedEventArgs(NeedsInvalidateVisualEvent));
		}

		private static void OnShouldInvalidateOnThemeChangeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is UIElement element)
			{
				var value = d.GetValue(ShouldInvalidateOnThemeChangeProperty);
				if (value == DependencyProperty.UnsetValue || (bool) value == false)
					RemoveNeedsInvalidateVisualHandler(element, OnNeedsInvalidate);
				else if ((bool) value)
					AddNeedsInvalidateVisualHandler(element, OnNeedsInvalidate);
			}
		}

		private static void OnNeedsInvalidate(object sender, RoutedEventArgs e)
		{
			if (sender is UIElement element)
			{
				element.InvalidateVisual();
				element.InvalidateArrange();
			}
		}


		private sealed class ThemeResourceDictionary : ResourceDictionary
		{
		}
	}
}
