using System;

namespace GoldenAnvil.Utility
{
	public static class DisposableUtility
	{
		public static void Dispose<T>(ref T obj)
			where T : IDisposable
		{
			obj?.Dispose();
			obj = default(T);
		}
	}
}
