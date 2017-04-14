using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace GoldenAnvil.Utility.Views
{
	public class FontSizeExtension : MarkupExtension
	{
		[TypeConverter(typeof(FontSizeConverter))]
		public double Size { get; set; }

		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return Size;
		}
	}
}
