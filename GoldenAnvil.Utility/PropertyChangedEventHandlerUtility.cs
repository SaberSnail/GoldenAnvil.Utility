using System.ComponentModel;

namespace GoldenAnvil.Utility
{
	public static class PropertyChangedEventHandlerUtility
	{
		public static void Raise(this PropertyChangedEventHandler eventHandler, object sender, string propertyName)
		{
			if (eventHandler != null)
				eventHandler(sender, new PropertyChangedEventArgs(propertyName));
		}
	}
}
