using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace GoldenAnvil.Utility.Windows
{
	public class TreeViewSelectionBehavior : Behavior<TreeView>
	{
		public TreeViewSelectionBehavior()
		{
			m_treeViewItemEventSetter = new EventSetter(
				FrameworkElement.LoadedEvent,
				new RoutedEventHandler(OnTreeViewItemLoaded)
				);
		}

		public delegate bool IsDescendantDelegate(object item, object descendentCandidate);

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyPropertyUtility<TreeViewSelectionBehavior>.Register(x => x.SelectedItem, OnSelectedItemChanged, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null);

		public object SelectedItem
		{
			get => GetValue(SelectedItemProperty);
			set => SetValue(SelectedItemProperty, value);
		}

		public static readonly DependencyProperty IsDescendantPredicateProperty =
			DependencyPropertyUtility<TreeViewSelectionBehavior>.Register(x => x.IsDescendantPredicate, null);

		public IsDescendantDelegate IsDescendantPredicate
		{
			get => (IsDescendantDelegate) GetValue(IsDescendantPredicateProperty);
			set => SetValue(IsDescendantPredicateProperty, value);
		}

		public static readonly DependencyProperty ExpandSelectedProperty =
			DependencyPropertyUtility<TreeViewSelectionBehavior>.Register(x => x.ExpandSelected, false);

		public bool ExpandSelected
		{
			get => (bool) GetValue(ExpandSelectedProperty);
			set => SetValue(ExpandSelectedProperty, value);
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
			((INotifyCollectionChanged) AssociatedObject.Items).CollectionChanged += OnTreeViewItemsChanged;

			UpdateTreeViewItemStyle();
			m_modelHandled = true;
			UpdateAllTreeViewItems();
			m_modelHandled = false;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			if (AssociatedObject is not null)
			{
				AssociatedObject.ItemContainerStyle?.Setters?.Remove(m_treeViewItemEventSetter);
				AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
				((INotifyCollectionChanged) AssociatedObject.Items).CollectionChanged -= OnTreeViewItemsChanged;
			}
		}

		private void OnTreeViewItemsChanged(object sender, NotifyCollectionChangedEventArgs args) =>
			UpdateAllTreeViewItems();

		private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> args)
		{
			if (m_modelHandled || AssociatedObject.Items.SourceCollection is null)
				return;

			SelectedItem = args.NewValue;
		}

		private void UpdateTreeViewItemStyle()
		{
			if (AssociatedObject.ItemContainerStyle is null)
			{
				var defaultStyle = Application.Current.TryFindResource(typeof(TreeViewItem)) as Style;
				AssociatedObject.ItemContainerStyle = new Style(typeof(TreeViewItem), defaultStyle);
			}

			if (!AssociatedObject.ItemContainerStyle.Setters.Contains(m_treeViewItemEventSetter))
				AssociatedObject.ItemContainerStyle.Setters.Add(m_treeViewItemEventSetter);
		}

		private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			var behavior = (TreeViewSelectionBehavior) sender;
			if (behavior.m_modelHandled || behavior.AssociatedObject is null)
				return;

			behavior.m_modelHandled = true;
			behavior.UpdateAllTreeViewItems();
			behavior.m_modelHandled = false;
		}

		private void UpdateAllTreeViewItems()
		{
			var treeView = AssociatedObject;
			foreach (var item in treeView.Items)
			{
				if (treeView.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem childItem)
					UpdateTreeViewItem(childItem, true);
			}
		}

		private void UpdateTreeViewItem(TreeViewItem item, bool recurse)
		{
			if (SelectedItem is null)
				return;

			var model = item.DataContext;

			if (SelectedItem == model && !item.IsSelected)
			{
				item.IsSelected = true;
				if (ExpandSelected)
					item.IsExpanded = true;
			}
			else
			{
				if (IsDescendantPredicate?.Invoke(model, SelectedItem) ?? true)
					item.IsExpanded = true;
			}

			if (recurse)
			{
				foreach (var subitem in item.Items)
				{
					if (item.ItemContainerGenerator.ContainerFromItem(subitem) is TreeViewItem childItem)
						UpdateTreeViewItem(childItem, true);
				}
			}
		}

		private void OnTreeViewItemLoaded(object sender, RoutedEventArgs args) =>
			UpdateTreeViewItem((TreeViewItem) sender, false);

		private readonly EventSetter m_treeViewItemEventSetter;
		private bool m_modelHandled;
	}
}
