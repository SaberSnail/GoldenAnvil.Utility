using System;

namespace GoldenAnvil.Utility
{
	public static class EventHandlerUtility
	{
		public static void Raise(this EventHandler eventHandler, object sender)
		{
			eventHandler?.Invoke(sender, EventArgs.Empty);
		}

		public static void Raise<T>(this EventHandler<T> eventHandler, object sender, T e) where T : EventArgs
		{
			eventHandler?.Invoke(sender, e);
		}
	}
}
