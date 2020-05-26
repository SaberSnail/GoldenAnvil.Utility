using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldenAnvil.Utility
{
	public static class EnumUtility
	{
		public static T[] Values<T>() where T : struct => (T[]) Enum.GetValues(typeof(T));
	}
}
