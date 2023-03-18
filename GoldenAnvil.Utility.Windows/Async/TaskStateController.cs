using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Threading;

namespace GoldenAnvil.Utility.Windows.Async
{
	public sealed class TaskStateController
	{
		public TaskStateController(CancellationToken token)
		{
			m_syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			m_cancellationToken = token;
		}

		public async Task ToSyncContext()
		{
			m_cancellationToken.ThrowIfCancellationRequested();
			await m_syncContextScheduler;
			m_cancellationToken.ThrowIfCancellationRequested();
		}

		public async Task ToThreadPool()
		{
			m_cancellationToken.ThrowIfCancellationRequested();
			await TaskScheduler.Default;
			m_cancellationToken.ThrowIfCancellationRequested();
		}

		public CancellationToken CancellationToken => m_cancellationToken;

		public void ThrowIfCancelled() => m_cancellationToken.ThrowIfCancellationRequested();

		private readonly TaskScheduler m_syncContextScheduler;
		private readonly CancellationToken m_cancellationToken;
	}
}
