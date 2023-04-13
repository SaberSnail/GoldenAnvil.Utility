using System.Threading;
using System.Threading.Tasks;

namespace GoldenAnvil.Utility.Windows.Async
{
	public sealed class TaskStateController
	{
		public TaskStateController(CancellationToken token)
		{
			m_syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			m_cancellationToken = token;
		}

		/// <summary>
		/// This can be awaited in order to switch to the sync context.
		/// The ability to await a TaskScheduler requires Microsoft.VisualStudio.Threading.
		/// </summary>
		/// <returns>A TaskScheduler that can be awaited.</returns>
		public TaskScheduler ToSyncContext()
		{
			m_cancellationToken.ThrowIfCancellationRequested();
			return m_syncContextScheduler;
		}

		/// <summary>
		/// This can be awaited in order to switch to the thread pool context.
		/// The ability to await a TaskScheduler requires Microsoft.VisualStudio.Threading.
		/// </summary>
		/// <returns>A TaskScheduler that can be awaited.</returns>
		public TaskScheduler ToThreadPool()
		{
			m_cancellationToken.ThrowIfCancellationRequested();
			return TaskScheduler.Default;
		}

		public CancellationToken CancellationToken => m_cancellationToken;

		public void ThrowIfCancelled() => m_cancellationToken.ThrowIfCancellationRequested();

		private readonly TaskScheduler m_syncContextScheduler;
		private readonly CancellationToken m_cancellationToken;
	}
}
