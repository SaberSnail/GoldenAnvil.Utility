using System.Windows;

namespace GoldenAnvil.Utility.Windows
{
	public static class FreezableUtility
	{
		public static T Frozen<T>(this T obj)
			where T : Freezable
		{
			obj.Freeze();
			return obj;
		}
	}
}
