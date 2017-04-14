using System.ComponentModel;

namespace GoldenAnvil.Utility
{
	public static class PropertyChangedEventHandlerUtility
	{
		public static void Raise(this PropertyChangedEventHandler eventHandler, object sender, string propertyName)
		{
			eventHandler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
		}
	}
}
