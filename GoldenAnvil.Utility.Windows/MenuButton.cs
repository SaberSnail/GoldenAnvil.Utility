using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace GoldenAnvil.Utility.Views
{
	public sealed class MenuButton : ToggleButton
	{
		public MenuButton()
		{
			Binding binding = new Binding
			{
				Path = new PropertyPath("Menu.IsOpen"),
				Source = this
			};
			SetBinding(IsCheckedProperty, binding);
		}

		public static readonly DependencyProperty MenuProperty = DependencyPropertyUtility<MenuButton>.Register(x => x.Menu);

		public ContextMenu Menu
		{
			get { return (ContextMenu) GetValue(MenuProperty); }
			set { SetValue(MenuProperty, value); }
		}

		protected override void OnClick()
		{
			if (Menu != null)
			{
				Menu.PlacementTarget = this;
				Menu.Placement = PlacementMode.Bottom;
				Menu.IsOpen = true;
			}
		}
	}
}
