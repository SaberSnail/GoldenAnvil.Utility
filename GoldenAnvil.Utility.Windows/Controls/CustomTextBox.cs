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

		public static readonly DependencyProperty HintProperty = DependencyPropertyUtility<CustomTextBox>.Register(x => x.Hint);

		public string Hint
		{
			get { return (string) GetValue(HintProperty); }
			set { SetValue(HintProperty, value); }
		}
	}
}
