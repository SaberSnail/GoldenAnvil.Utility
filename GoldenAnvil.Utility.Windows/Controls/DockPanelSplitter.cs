#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows;

namespace GoldenAnvil.Utility.Windows.Controls;

public class DockPanelSplitter : Thumb
{
	public static readonly IValueConverter CursorConverter = new CursorConverterImpl();

	static DockPanelSplitter()
	{
		DefaultStyleKeyProperty.OverrideMetadata(typeof(DockPanelSplitter), new FrameworkPropertyMetadata(typeof(DockPanelSplitter)));
	}

	public DockPanelSplitter()
	{
		m_target = s_targetNullObject;

		Loaded += OnLoaded;
		MouseDoubleClick += OnMouseDoubleClick;
		DragStarted += OnDragStarted;
		DragDelta += OnDragDelta;
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		if (Parent is not DockPanel)
			throw new InvalidOperationException($"{nameof(DockPanelSplitter)} must be directly in a DockPanel.");

		if (GetTargetOrDefault() is null)
			throw new InvalidOperationException($"{nameof(DockPanelSplitter)} must be directly after a FrameworkElement");
	}

	private void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
	{
		if (m_initialLength is not null)
			SetTargetLength(m_initialLength.Value);
	}

	private void OnDragStarted(object sender, DragStartedEventArgs e)
	{
		m_isHorizontal = GetIsHorizontal(this);
		m_isBottomOrRight = GetIsBottomOrRight();
		m_target = GetTargetOrDefault() ?? s_targetNullObject;
		m_initialLength ??= GetTargetLength();
		m_availableSpace = GetAvailableSpace();
	}

	private void OnDragDelta(object sender, DragDeltaEventArgs e)
	{
		var change = m_isHorizontal ? e.VerticalChange : e.HorizontalChange;
		if (m_isBottomOrRight)
			change = -change;

		var targetLength = GetTargetLength();
		var newTargetLength = targetLength + change;
		newTargetLength = Clamp(newTargetLength, 0, m_availableSpace);
		newTargetLength = Math.Round(newTargetLength);

		SetTargetLength(newTargetLength);
	}

	private FrameworkElement? GetTargetOrDefault()
	{
		var children = ParentDockPanel.Children.OfType<object>();
		var splitterIndex = ParentDockPanel.Children.IndexOf(this);
		return children.ElementAtOrDefault(splitterIndex - 1) as FrameworkElement;
	}

	private bool GetIsBottomOrRight()
	{
		var position = DockPanel.GetDock(this);
		return position == Dock.Bottom || position == Dock.Right;
	}

	private double GetAvailableSpace()
	{
		var lastChild = ParentDockPanel.LastChildFill ?
			ParentDockPanel.Children.OfType<object>().Last() as FrameworkElement :
			null;

		var fixedChildren = ParentDockPanel.Children.OfType<FrameworkElement>()
			.Where(x => GetIsHorizontal(x) == m_isHorizontal && x != m_target && x != lastChild);

		var panelLength = GetLength(ParentDockPanel);
		var unavailableSpace = fixedChildren.Sum(c => GetLength(c));
		return panelLength - unavailableSpace;
	}

	private void SetTargetLength(double length)
	{
		if (m_isHorizontal)
			m_target.Height = length;
		else
			m_target.Width = length;
	}

	private double GetTargetLength() => GetLength(m_target);

	private static bool GetIsHorizontal(FrameworkElement element)
	{
		var position = DockPanel.GetDock(element);
		return GetIsHorizontal(position);
	}

	private static bool GetIsHorizontal(Dock position)
			=> position == Dock.Top || position == Dock.Bottom;

	private static double Clamp(double value, double min, double max)
			=> value < min ? min :
				 value > max ? max :
				 value;

	private double GetLength(FrameworkElement element)
			=> m_isHorizontal ?
				 element.ActualHeight :
				 element.ActualWidth;

	private DockPanel ParentDockPanel => Parent as DockPanel ?? s_parentNullObject;

	private class CursorConverterImpl : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var position = (Dock) value;
			var isHorizontal = GetIsHorizontal(position);
			return isHorizontal ? Cursors.SizeNS : Cursors.SizeWE;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
				=> throw new NotImplementedException();
	}

	private static readonly FrameworkElement s_targetNullObject = new();
	private static readonly DockPanel s_parentNullObject = new();

	private bool m_isHorizontal;
	private bool m_isBottomOrRight;
	private FrameworkElement m_target;
	private double? m_initialLength;
	private double m_availableSpace;
}
