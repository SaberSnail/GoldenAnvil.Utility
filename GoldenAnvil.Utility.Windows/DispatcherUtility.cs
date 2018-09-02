using System;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace GoldenAnvil.Utility.Windows
{
	public static class DispatcherUtility
	{
		public static void VerifyCurrent(this Dispatcher dispatcher, [CallerMemberName] string callerName = "")
		{
			if (dispatcher != Dispatcher.CurrentDispatcher)
				throw new InvalidOperationException($"{callerName} must be running on the dispatcher thread.");
		}

		public static void VerifyNotCurrent(this Dispatcher dispatcher, [CallerMemberName] string callerName = "")
		{
			if (dispatcher == Dispatcher.CurrentDispatcher)
				throw new InvalidOperationException($"{callerName} must not be running on the dispatcher thread.");
		}
	}
}
