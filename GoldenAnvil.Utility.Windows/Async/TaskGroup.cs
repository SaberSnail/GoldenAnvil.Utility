using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.VisualStudio.Threading;

namespace GoldenAnvil.Utility.Windows.Async
{
	public sealed class TaskGroup : IDisposable
	{
		public TaskGroup()
		{
			m_dispatcher = Dispatcher.CurrentDispatcher;
			m_syncContextScheduler = TaskScheduler.FromCurrentSynchronizationContext();
			m_tokenSources = new List<CancellationTokenSource>();
		}

		public void Dispose()
		{
			foreach (var tokenSource in m_tokenSources)
			{
				tokenSource.Cancel();
				tokenSource.Dispose();
			}
		}

		internal async Task RegisterTaskAsync(Func<CancellationToken, Task> task)
		{
			m_dispatcher.VerifyCurrent();
			var tokenSource = new CancellationTokenSource();
			m_tokenSources.Add(tokenSource);

			try
			{
				await task(tokenSource.Token).ConfigureAwait(false);
			}
			catch (OperationCanceledException e) when (e.CancellationToken == tokenSource.Token)
			{
			}
			finally
			{
				await m_syncContextScheduler;
				m_tokenSources.Remove(tokenSource);
			}
		}

		internal async Task<T> RegisterTaskAsync<T>(Func<CancellationToken, Task<T>> task)
		{
			m_dispatcher.VerifyCurrent();
			var tokenSource = new CancellationTokenSource();
			m_tokenSources.Add(tokenSource);

			T result = default;
			try
			{
				result = await task(tokenSource.Token).ConfigureAwait(false);
			}
			catch (OperationCanceledException e) when (e.CancellationToken == tokenSource.Token)
			{
			}
			finally
			{
				await m_syncContextScheduler;
				m_tokenSources.Remove(tokenSource);
			}
			return result;
		}

		private readonly Dispatcher m_dispatcher;
		private readonly TaskScheduler m_syncContextScheduler;
		private readonly List<CancellationTokenSource> m_tokenSources;
	}
}
