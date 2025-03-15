using System;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows;
using System.Globalization;
using System.Windows.Controls;

namespace GoldenAnvil.Utility.Windows
{
	public static class TextElementUtility
	{
		public static readonly DependencyProperty TextProperty =
	DependencyProperty.RegisterAttached("Text", typeof(IEnumerable<Inline>), typeof(TextElementUtility), new PropertyMetadata(null, OnTextChanged));

		public static IEnumerable<Inline> GetText(DependencyObject dependencyObject) =>
			(IEnumerable<Inline>) dependencyObject.GetValue(TextProperty);

		public static void SetText(DependencyObject dependencyObject, IEnumerable<Inline> value) =>
			dependencyObject.SetValue(TextProperty, value);

		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is TextBlock textBlock)
			{
				textBlock.Inlines.Clear();
				var textElements = GetText(d);
				foreach (var textElement in textElements.EmptyIfNull())
					textBlock.Inlines.Add(textElement);
				return;
			}

			throw new InvalidOperationException($"The target of the Text property is not of a supported type: {d.GetType().Name}");
		}

		public static IReadOnlyList<Inline> FormatInlineString(string format, string styleName, params Inline[] inlines)
		{
			var style = styleName is null ? null : (Style) Application.Current.FindResource(styleName);
			var formattedInlines = new List<Inline>();

			int startIndex = 0;
			while (true)
			{
				int openBraceIndex = format.IndexOf('{', startIndex);
				int closeBraceIndex = format.IndexOf('}', startIndex);
				if (closeBraceIndex < openBraceIndex || openBraceIndex == -1)
				{
					// No more format specifiers, add remaining text to the output
					string remainingText = format.Substring(startIndex);
					if (!string.IsNullOrEmpty(remainingText))
					{
						var run = new Run(remainingText);
						run.Style = style;
						formattedInlines.Add(run);
					}
					break;
				}

				// Check for escaped braces
				if (openBraceIndex > 0 && format[openBraceIndex - 1] == '{')
				{
					// This is an escaped opening brace, add the text up to the brace and continue searching from after the brace
					var run = new Run(format.Substring(startIndex, openBraceIndex - startIndex - 1) + "{");
					run.Style = style;
					formattedInlines.Add(run);
					startIndex = openBraceIndex + 1;
					continue;
				}

				if (closeBraceIndex < format.Length - 1 && format[closeBraceIndex + 1] == '}')
				{
					// This is an escaped closing brace, add the text up to the brace and continue searching from after the brace
					var run = new Run(format.Substring(startIndex, closeBraceIndex - startIndex - 1) + "}");
					run.Style = style;
					formattedInlines.Add(run);
					startIndex = closeBraceIndex + 2;
					continue;
				}

				// Parse the format specifier
				int index;
				if (int.TryParse(format.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1), NumberStyles.Integer, CultureInfo.InvariantCulture, out index) && index >= 0 && index < inlines.Length)
				{
					// Add the text up to the opening brace
					if (openBraceIndex > startIndex)
					{
						var run = new Run(format.Substring(startIndex, openBraceIndex - startIndex));
						run.Style = style;
						formattedInlines.Add(run);
					}

					// Add the inline element
					formattedInlines.Add(inlines[index]);

					startIndex = closeBraceIndex + 1;
				}
				else
				{
					throw new FormatException("Invalid format string: " + format);
				}
			}

			return formattedInlines.AsReadOnly();
		}

		public static T WithStyle<T>(this T element, string styleName)
			where T : TextElement
		{
			element.Style = (Style) Application.Current.FindResource(styleName);
			return element;
		}
	}
}
