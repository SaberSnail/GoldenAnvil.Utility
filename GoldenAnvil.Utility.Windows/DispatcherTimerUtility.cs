using System.Windows.Threading;

namespace GoldenAnvil.Utility.Windows;

public static class DispatcherTimerUtility
{
	public static void Restart(this DispatcherTimer timer)
	{
		timer.Stop();
		timer.Start();
	}
}
