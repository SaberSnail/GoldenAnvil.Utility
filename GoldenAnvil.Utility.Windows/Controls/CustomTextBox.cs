using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GoldenAnvil.Utility.Windows.Controls
{
	public class CustomTextBox : TextBox
	{
		static CustomTextBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTextBox), new FrameworkPropertyMetadata(typeof(CustomTextBox)));
		}

		public static readonly DependencyProperty HintTextProperty = DependencyPropertyUtility<CustomTextBox>.Register(x => x.HintText);

		public string HintText
		{
			get => (string) GetValue(HintTextProperty);
			set => SetValue(HintTextProperty, value);
		}

		public static readonly DependencyProperty ForceTextSourceUpdateOnEnterProperty = DependencyPropertyUtility<CustomTextBox>.Register(x => x.ForceTextSourceUpdateOnEnter, new PropertyChangedCallback(OnForceTextSourceUpdateOnEnterChanged));

		public bool ForceTextSourceUpdateOnEnter
		{
			get => (bool) GetValue(ForceTextSourceUpdateOnEnterProperty);
			set => SetValue(ForceTextSourceUpdateOnEnterProperty, value);
		}

		private static void OnForceTextSourceUpdateOnEnterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var textBox = (CustomTextBox) d;
			if (textBox.ForceTextSourceUpdateOnEnter)
				textBox.KeyDown += OnTextBoxKeyDown;
			else
				textBox.KeyDown -= OnTextBoxKeyDown;
		}

		private static void OnTextBoxKeyDown(object sender, KeyEventArgs e)
		{
			var textBox = (CustomTextBox) sender;
			if (e.Key == Key.Enter)
			{
				var expression = textBox.GetBindingExpression(TextBox.TextProperty);
				expression?.UpdateSource();
			}
		}
	}
}
