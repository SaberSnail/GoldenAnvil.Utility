using System.ComponentModel;

namespace GoldenAnvil.Utility
{
	public static class PropertyChangingEventHandlerUtility
	{
		public static void Raise(this PropertyChangingEventHandler eventHandler, object sender, string propertyName)
		{
			if (eventHandler != null)
				eventHandler(sender, new PropertyChangingEventArgs(propertyName));
		}
	}
}
