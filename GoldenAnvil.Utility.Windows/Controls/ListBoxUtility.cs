using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GoldenAnvil.Utility.Windows.Controls;

public static class ListBoxUtility
{
	public static readonly DependencyProperty SelectedItemsProperty =
		DependencyProperty.RegisterAttached("SelectedItems", typeof(IList), typeof(ListBoxUtility), new PropertyMetadata(null, OnSelectedItemsChanged));

	public static IList GetSelectedItems(DependencyObject dependencyObject) =>
		(IList) dependencyObject.GetValue(SelectedItemsProperty);

	public static void SetSelectedItems(DependencyObject dependencyObject, IList value) =>
		dependencyObject.SetValue(SelectedItemsProperty, value);

	private static readonly DependencyProperty SelectedItemsSynchronizerProperty =
		DependencyProperty.RegisterAttached("SelectedItemsSynchronizer", typeof(SynchronizationManager), typeof(ListBoxUtility));

	private static SynchronizationManager GetSelectedItemsSynchronizer(DependencyObject dependencyObject) =>
		(SynchronizationManager) dependencyObject.GetValue(SelectedItemsSynchronizerProperty);

	private static void SetSelectedItemsSynchronizer(DependencyObject dependencyObject, SynchronizationManager value) =>
		dependencyObject.SetValue(SelectedItemsSynchronizerProperty, value);

	private static void OnSelectedItemsChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
	{
		if (e.OldValue is not null)
		{
			var synchronizer = GetSelectedItemsSynchronizer(dependencyObject);
			synchronizer.StopSynchronizing();
			SetSelectedItemsSynchronizer(dependencyObject, null);
		}

		// check that this property is an IList, and that it is being set on a ListBox
		if (e.NewValue is IList && dependencyObject is Selector selector)
		{
			SynchronizationManager synchronizer = GetSelectedItemsSynchronizer(dependencyObject);
			if (synchronizer is null)
			{
				synchronizer = new SynchronizationManager(selector);
				SetSelectedItemsSynchronizer(dependencyObject, synchronizer);
			}

			synchronizer.StartSynchronizingList();
		}
	}

	private class SynchronizationManager
	{
		internal SynchronizationManager(Selector selector)
		{
			m_multiSelector = selector;
		}

		public void StartSynchronizingList()
		{
			var list = GetSelectedItems(m_multiSelector);

			if (list is not null)
			{
				m_synchronizer = new TwoListSynchronizer(GetSelectedItemsCollection(m_multiSelector), list);
				m_synchronizer.StartSynchronizing();
			}
		}

		public void StopSynchronizing() => m_synchronizer.StopSynchronizing();

		public static IList GetSelectedItemsCollection(Selector selector)
		{
			if (selector is MultiSelector)
				return (selector as MultiSelector).SelectedItems;
			else if (selector is ListBox)
				return (selector as ListBox).SelectedItems;
			else
				throw new InvalidOperationException("Target object has no SelectedItems property to bind.");
		}

		private readonly Selector m_multiSelector;
		private TwoListSynchronizer m_synchronizer;
	}

	public class TwoListSynchronizer : IWeakEventListener
	{
		public TwoListSynchronizer(IList masterList, IList targetList)
		{
			m_masterList = masterList;
			m_targetList = targetList;
		}

		public void StartSynchronizing()
		{
			ListenForChangeEvents(m_masterList);
			ListenForChangeEvents(m_targetList);

			SetListValuesFromSource(m_masterList, m_targetList);

			if (!TargetAndMasterCollectionsAreEqual())
				SetListValuesFromSource(m_targetList, m_masterList);
		}

		public void StopSynchronizing()
		{
			StopListeningForChangeEvents(m_masterList);
			StopListeningForChangeEvents(m_targetList);
		}

		public bool ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			HandleCollectionChanged(sender as IList, e as NotifyCollectionChangedEventArgs);
			return true;
		}

		protected void ListenForChangeEvents(IList list)
		{
			if (list is INotifyCollectionChanged)
				CollectionChangedEventManager.AddListener(list as INotifyCollectionChanged, this);
		}

		protected void StopListeningForChangeEvents(IList list)
		{
			if (list is INotifyCollectionChanged)
				CollectionChangedEventManager.RemoveListener(list as INotifyCollectionChanged, this);
		}

		private void AddItems(IList list, NotifyCollectionChangedEventArgs e)
		{
			int itemCount = e.NewItems.Count;

			for (int i = 0; i < itemCount; i++)
			{
				int insertionPoint = e.NewStartingIndex + i;

				if (insertionPoint > list.Count)
					list.Add(e.NewItems[i]);
				else
					list.Insert(insertionPoint, e.NewItems[i]);
			}
		}

		private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			IList sourceList = sender as IList;

			switch (e.Action)
			{
			case NotifyCollectionChangedAction.Add:
				PerformActionOnAllLists(AddItems, sourceList, e);
				break;
			case NotifyCollectionChangedAction.Move:
				PerformActionOnAllLists(MoveItems, sourceList, e);
				break;
			case NotifyCollectionChangedAction.Remove:
				PerformActionOnAllLists(RemoveItems, sourceList, e);
				break;
			case NotifyCollectionChangedAction.Replace:
				PerformActionOnAllLists(ReplaceItems, sourceList, e);
				break;
			case NotifyCollectionChangedAction.Reset:
				UpdateListsFromSource(sender as IList);
				break;
			default:
				break;
			}
		}

		private void MoveItems(IList list, NotifyCollectionChangedEventArgs e)
		{
			RemoveItems(list, e);
			AddItems(list, e);
		}

		private void PerformActionOnAllLists(ChangeListAction action, IList sourceList, NotifyCollectionChangedEventArgs collectionChangedArgs)
		{
			if (sourceList == m_masterList)
				PerformActionOnList(m_targetList, action, collectionChangedArgs);
			else
				PerformActionOnList(m_masterList, action, collectionChangedArgs);
		}

		private void PerformActionOnList(IList list, ChangeListAction action, NotifyCollectionChangedEventArgs collectionChangedArgs)
		{
			StopListeningForChangeEvents(list);
			action(list, collectionChangedArgs);
			ListenForChangeEvents(list);
		}

		private void RemoveItems(IList list, NotifyCollectionChangedEventArgs e)
		{
			int itemCount = e.OldItems.Count;
			for (int i = 0; i < itemCount; i++)
				list.RemoveAt(e.OldStartingIndex);
		}

		private void ReplaceItems(IList list, NotifyCollectionChangedEventArgs e)
		{
			RemoveItems(list, e);
			AddItems(list, e);
		}

		private void SetListValuesFromSource(IList sourceList, IList targetList)
		{
			StopListeningForChangeEvents(targetList);

			targetList.Clear();

			foreach (object o in sourceList)
				targetList.Add(o);

			ListenForChangeEvents(targetList);
		}

		private bool TargetAndMasterCollectionsAreEqual() =>
			m_masterList.Cast<object>().SequenceEqual(m_targetList.Cast<object>());

		private void UpdateListsFromSource(IList sourceList)
		{
			if (sourceList == m_masterList)
				SetListValuesFromSource(m_masterList, m_targetList);
			else
				SetListValuesFromSource(m_targetList, m_masterList);
		}

		private delegate void ChangeListAction(IList list, NotifyCollectionChangedEventArgs e);

		private readonly IList m_masterList;
		private readonly IList m_targetList;
	}
}
