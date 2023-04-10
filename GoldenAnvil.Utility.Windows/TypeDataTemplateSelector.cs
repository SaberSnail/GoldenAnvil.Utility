using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace GoldenAnvil.Utility.Windows
{
	[ContentProperty("Templates")]
	public sealed class TypeDataTemplateSelector : DataTemplateSelector
	{
		public TypeDataTemplateSelector()
		{
			Templates = new Collection<TypeDataTemplate>();
		}

		public Collection<TypeDataTemplate> Templates { get; }

		public override DataTemplate SelectTemplate(object item, DependencyObject container)
		{
			if (item is null)
				return null;

			var itemType = item.GetType();
			return Templates
				.Where(x => itemType.IsSameOrSubclassOf(x.Type))
				.Select(x => x.Template)
				.FirstOrDefault();
		}
	}

	[ContentProperty("Template")]
	public sealed class TypeDataTemplate
	{
		public Type Type { get; set; }
		public DataTemplate Template { get; set; }
	}
}
