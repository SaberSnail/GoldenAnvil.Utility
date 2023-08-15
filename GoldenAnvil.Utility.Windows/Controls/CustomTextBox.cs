using System.Windows;
using System.Windows.Controls;

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
			get { return (string) GetValue(HintTextProperty); }
			set { SetValue(HintTextProperty, value); }
		}
	}
}
