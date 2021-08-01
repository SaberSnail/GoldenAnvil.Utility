using System.ComponentModel;

namespace GoldenAnvil.Utility
{
	public static class PropertyChangedEventHandlerUtility
	{
		public static void Raise(this PropertyChangedEventHandler eventHandler, object sender, string propertyName)
		{
			eventHandler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
		}

		public static bool HasChanged(this PropertyChangedEventArgs e, string propertyName)
		{
			var eventPropertyName = e.PropertyName;
			return string.IsNullOrEmpty(eventPropertyName) || propertyName == eventPropertyName;
		}
	}
}
