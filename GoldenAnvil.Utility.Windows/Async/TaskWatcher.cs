using System;
using System.Threading;
using System.Threading.Tasks;

namespace GoldenAnvil.Utility.Windows.Async
{
	public sealed class TaskWatcher : NotifyPropertyChangedDispatcherBase
	{
		public static TaskWatcher Create(Func<TaskStateController, Task> doTask, TaskGroup group)
		{
			var tokenSource = new CancellationTokenSource();
			var task = group.RegisterTaskAsync(doTask, tokenSource.Token);
			return new(task, tokenSource);
		}

		public static TaskWatcher<T> Create<T>(Func<TaskStateController, Task<T>> doTask, TaskGroup group, T defaultResult = default)
		{
			var tokenSource = new CancellationTokenSource();
			var task = group.RegisterTaskAsync(doTask, tokenSource.Token);
			return new(task, tokenSource, defaultResult);
		}

		public Task Task { get; }

		public Task TaskCompleted { get; }

		public TaskStatus Status => Task.Status;

		public bool IsCompleted => Task.IsCompleted;

		public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

		public bool IsCanceled => Task.IsCanceled;

		public bool IsFaulted => Task.IsFaulted;

		public AggregateException Exception => Task.Exception;

		public Exception InnerException => Exception?.InnerException;

		public string ErrorMessage => InnerException?.Message;

		public void Cancel() => m_tokenSource.Cancel();

		internal CancellationToken Token => m_tokenSource.Token;

		private TaskWatcher(Task task, CancellationTokenSource tokenSource)
		{
			m_tokenSource = tokenSource;
			Task = task;
			TaskCompleted = WatchTaskAsync(task);
		}

		private async Task WatchTaskAsync(Task task)
		{
			try
			{
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
				await task;
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
			}
			catch
			{
				// ignore exception
			}
			finally
			{
				RaiseChangedProperties(task);
			}
		}

		private void RaiseChangedProperties(Task task)
		{
			if (task.IsCanceled)
				ScopedPropertyChange(nameof(Status), nameof(IsCanceled), nameof(IsCompleted)).Dispose();
			else if (task.IsFaulted)
				ScopedPropertyChange(nameof(Exception), nameof(InnerException), nameof(ErrorMessage), nameof(Status), nameof(IsFaulted), nameof(IsCompleted)).Dispose();
			else
				ScopedPropertyChange(nameof(Status), nameof(IsSuccessfullyCompleted), nameof(IsCompleted));
		}

		private readonly CancellationTokenSource m_tokenSource;
	}

	public sealed class TaskWatcher<T> : NotifyPropertyChangedDispatcherBase
	{
		public Task<T> Task { get; }

		public Task TaskCompleted { get; }

#pragma warning disable VSTHRD002 // Avoid problematic synchronous waits
#pragma warning disable VSTHRD104 // Offer async methods
		public T Result => (Task.Status == TaskStatus.RanToCompletion) ? Task.Result : m_defaultResult;
#pragma warning restore VSTHRD104 // Offer async methods
#pragma warning restore VSTHRD002 // Avoid problematic synchronous waits

		public TaskStatus Status => Task.Status;

		public bool IsCompleted => Task.IsCompleted;

		public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

		public bool IsCanceled => Task.IsCanceled;

		public bool IsFaulted => Task.IsFaulted;

		public AggregateException Exception => Task.Exception;

		public Exception InnerException => Exception?.InnerException;

		public string ErrorMessage => InnerException?.Message;

		public void Cancel() => m_tokenSource.Cancel();

		internal CancellationToken Token => m_tokenSource.Token;

		internal TaskWatcher(Task<T> task, CancellationTokenSource tokenSource, T defaultResult)
		{
			m_defaultResult = defaultResult;
			m_tokenSource = tokenSource;
			Task = task;
			TaskCompleted = WatchTaskAsync(task);
		}

		private async Task WatchTaskAsync(Task task)
		{
			try
			{
#pragma warning disable VSTHRD003 // Avoid awaiting foreign Tasks
				await task;
#pragma warning restore VSTHRD003 // Avoid awaiting foreign Tasks
			}
			catch
			{
				// ignore exception
			}
			finally
			{
				RaiseChangedProperties(task);
			}
		}

		private void RaiseChangedProperties(Task task)
		{
			if (task.IsCanceled)
				ScopedPropertyChange(nameof(Status), nameof(IsCanceled), nameof(IsCompleted)).Dispose();
			else if (task.IsFaulted)
				ScopedPropertyChange(nameof(Exception), nameof(InnerException), nameof(ErrorMessage), nameof(Status), nameof(IsFaulted), nameof(IsCompleted)).Dispose();
			else
				ScopedPropertyChange(nameof(Result), nameof(Status), nameof(IsSuccessfullyCompleted), nameof(IsCompleted));
		}

		private readonly CancellationTokenSource m_tokenSource;
		private readonly T m_defaultResult;
	}
}
