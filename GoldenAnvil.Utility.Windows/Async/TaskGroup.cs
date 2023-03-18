using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace GoldenAnvil.Utility.Windows.Async
{
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

		internal async Task RegisterTaskAsync(Func<TaskStateController, Task> doTask, CancellationToken token = default)
		{
			m_dispatcher.VerifyCurrent();
			var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(m_tokenSource.Token, token);
			var controller = new TaskStateController(linkedTokenSource.Token);
			
			try
			{
				await doTask(controller).ConfigureAwait(false);
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
