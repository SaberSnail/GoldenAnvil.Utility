using System;

namespace GoldenAnvil.Utility
{
	public sealed class Scope : IDisposable
	{
		public static readonly Scope Empty = new Scope(null);

		public static Scope Create(Action onDispose)
		{
			return new Scope(onDispose);
		}

		public static Scope Create<T>(T disposable) where T : IDisposable
		{
			return disposable == null ? Empty : new Scope(disposable.Dispose);
		}

		public void Cancel()
		{
			m_onDispose = null;
		}

		public void Dispose()
		{
			if (m_onDispose != null)
			{
				m_onDispose();
				m_onDispose = null;
			}
		}

		private Scope(Action onDispose)
		{
			m_onDispose = onDispose;
		}

		Action m_onDispose;
	}
}
