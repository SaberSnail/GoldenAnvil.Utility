using System;

namespace GoldenAnvil.Utility
{
	public sealed class GenericEventArgs<T> : EventArgs
	{
		public GenericEventArgs(T value)
		{
			Value = value;
		}

		public T Value { get; }
	}
}
