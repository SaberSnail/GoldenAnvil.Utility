using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
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

		public async Task ToSyncContext() => await m_syncContextScheduler;

		public async Task ToThreadPool() => await TaskScheduler.Default;

		public void ThrowIfCancelled() => m_cancellationToken.ThrowIfCancellationRequested();

		private readonly TaskScheduler m_syncContextScheduler;
		private readonly CancellationToken m_cancellationToken;
	}

	public sealed class TaskState : NotifyPropertyChangedDispatcherBase
	{
		public void Cancel()
		{}
	}
	public sealed class TaskGroup : IDisposable
	{
		public TaskGroup()
		{
			m_dispatcher = Dispatcher.CurrentDispatcher;
			m_tokenSource = new CancellationTokenSource();
		}

		public void Dispose()
		{
			m_tokenSource.Cancel();
			m_tokenSource.Dispose();
		}

		public async TaskState StartWork()
		{
			await RegisterTaskAsync();
		}

		internal async Task RegisterTaskAsync(Func<TaskStateController, Task> task, CancellationToken token = default)
		{
			m_dispatcher.VerifyCurrent();
			var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(m_tokenSource.Token, token);
			var controller = new TaskStateController(linkedTokenSource.Token);
			
			try
			{
				await task(controller).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
			}
		}

		internal async Task<T> RegisterTaskAsync<T>(Func<TaskStateController, Task<T>> task, CancellationToken token = default)
		{
			m_dispatcher.VerifyCurrent();
			var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(m_tokenSource.Token, token);
			var controller = new TaskStateController(linkedTokenSource.Token);

			T result = default;
			try
			{
				result = await task(controller).ConfigureAwait(false);
			}
			catch (OperationCanceledException)
			{
			}
			return result;
		}

		private readonly Dispatcher m_dispatcher;
		private readonly CancellationTokenSource m_tokenSource;
	}
}
