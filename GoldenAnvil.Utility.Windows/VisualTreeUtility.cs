using System.Collections.Generic;
using System.Windows.Media;
using System.Windows;

namespace GoldenAnvil.Utility.Windows
{
	public static class VisualTreeUtility
	{
		public static IEnumerable<DependencyObject> EnumerateDescendants(DependencyObject parent)
		{
			var stack = new Stack<DependencyObject>();
			stack.Push(parent);
			while (stack.Count != 0)
			{
				var current = stack.Pop();
				yield return current;
				for (var i = VisualTreeHelper.GetChildrenCount(current) - 1; i >= 0; --i)
					stack.Push(VisualTreeHelper.GetChild(current, i));
			}
		}

		public static T GetAncestor<T>(DependencyObject obj) where T : DependencyObject
		{
			obj = VisualTreeHelper.GetParent(obj);
			while (obj != null)
			{
				if (obj is T ancestor)
					return ancestor;
				obj = VisualTreeHelper.GetParent(obj);
			}
			return null;
		}
	}
}
