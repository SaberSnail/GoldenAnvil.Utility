using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows;

namespace GoldenAnvil.Utility.Windows
{
	[ContentProperty("Styles")]
	public sealed class TypeStyleSelector : StyleSelector
	{
		public TypeStyleSelector()
		{
			Styles = new Collection<TypeStyle>();
		}

		public Collection<TypeStyle> Styles { get; }

		public override Style SelectStyle(object item, DependencyObject container)
		{
			if (item is null)
				return null;

			var itemType = item.GetType();
			return Styles
				.Where(x => itemType.IsSameOrSubclassOf(x.Type))
				.Select(x => x.Style)
				.FirstOrDefault();
		}
	}

	[ContentProperty("Style")]
	public sealed class TypeStyle
	{
		public Type Type { get; set; }
		public Style Style { get; set; }
	}
}
