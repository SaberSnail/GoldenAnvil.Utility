﻿using System;
using System.Threading;
using System.Threading.Tasks;
using GoldenAnvil.Utility.Logging;

namespace GoldenAnvil.Utility.Windows.Async
{
	public sealed class TaskWatcher : NotifyPropertyChangedDispatcherBase
	{
		public static TaskWatcher Execute(Func<TaskStateController, Task> doTask, TaskGroup group)
		{
			var tokenSource = new CancellationTokenSource();
			var task = group.RegisterTaskAsync(doTask, tokenSource.Token);
			return new(task, tokenSource);
		}

		public static TaskWatcher<T> Execute<T>(Func<TaskStateController, Task<T>> doTask, TaskGroup group, T defaultResult = default)
		{
			var tokenSource = new CancellationTokenSource();
			var task = group.RegisterTaskAsync(doTask, tokenSource.Token);
			return new(task, tokenSource, defaultResult);
		}

		public static TaskWatcher Create(Func<TaskStateController, Task> doTask, TaskGroup group)
		{
			return new(doTask, group);
		}

		public Task Task { get; private set; }

		public Task TaskCompleted { get; private set; }

		public TaskStatus Status => Task.Status;

		public bool IsCompleted => Task.IsCompleted;

		public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

		public bool IsCanceled => Task.IsCanceled;

		public bool IsFaulted => Task.IsFaulted;

		public AggregateException Exception => Task.Exception;

		public Exception InnerException => Exception?.InnerException;

		public string ErrorMessage => InnerException?.Message;

		public void Start()
		{
			if (m_doTask is null || m_group is null)
				throw new InvalidOperationException("TaskWatcher can only be run once");

			var doTask = m_doTask;
			m_doTask = null;
			var group = m_group;
			m_group = null;

			m_tokenSource = new CancellationTokenSource();
			Task = group.RegisterTaskAsync(doTask, m_tokenSource.Token);
			TaskCompleted = WatchTaskAsync(Task);
		}

		public void Cancel() => m_tokenSource.Cancel();

		internal CancellationToken Token => m_tokenSource.Token;

		private TaskWatcher(Func<TaskStateController, Task> doTask, TaskGroup group)
		{
			m_doTask = doTask;
			m_group = group;
		}

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
			{
				RaisePropertyChanged(nameof(Status), nameof(IsCanceled), nameof(IsCompleted));
			}
			else if (task.IsFaulted)
			{
				if (!HasAnyPropertyChangedHandlers())
				{
					ThreadPool.QueueUserWorkItem(_ =>
					{
						if (Exception != null)
						{
							Log.Error($"Unhandled exception in Task:\n{Exception}");
							throw Exception;
						}
					});
				}
				RaisePropertyChanged(nameof(Exception), nameof(InnerException), nameof(ErrorMessage), nameof(Status), nameof(IsFaulted), nameof(IsCompleted));
			}
			else
			{
				RaisePropertyChanged(nameof(Status), nameof(IsSuccessfullyCompleted), nameof(IsCompleted));
			}
		}

		private static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(TaskWatcher));

		private Func<TaskStateController, Task> m_doTask;
		private TaskGroup m_group;
		private CancellationTokenSource m_tokenSource;
	}

	public sealed class TaskWatcher<T> : NotifyPropertyChangedDispatcherBase
	{
		public Task<T> Task { get; private set; }

		public Task TaskCompleted { get; private set; }

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

		public void Start()
		{
			if (m_doTask is null || m_group is null)
				throw new InvalidOperationException("TaskWatcher can only be run once");

			var doTask = m_doTask;
			m_doTask = null;
			var group = m_group;
			m_group = null;

			m_tokenSource = new CancellationTokenSource();
			Task = group.RegisterTaskAsync(doTask, m_tokenSource.Token);
			TaskCompleted = WatchTaskAsync(Task);
		}

		public void Cancel() => m_tokenSource.Cancel();

		internal CancellationToken Token => m_tokenSource.Token;

		private TaskWatcher(Func<TaskStateController, Task<T>> doTask, TaskGroup group)
		{
			m_doTask = doTask;
			m_group = group;
		}

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
			{
				RaisePropertyChanged(nameof(Status), nameof(IsCanceled), nameof(IsCompleted));
			}
			else if (task.IsFaulted)
			{
				if (!HasAnyPropertyChangedHandlers())
				{
					ThreadPool.QueueUserWorkItem(_ =>
					{
						if (Exception != null)
						{
							Log.Error($"Unhandled exception in Task<T>:\n{Exception}");
							throw Exception;
						}
					});
				}
				RaisePropertyChanged(nameof(Exception), nameof(InnerException), nameof(ErrorMessage), nameof(Status), nameof(IsFaulted), nameof(IsCompleted));
			}
			else
			{
				RaisePropertyChanged(nameof(Result), nameof(Status), nameof(IsSuccessfullyCompleted), nameof(IsCompleted));
			}
		}

		private static ILogSource Log { get; } = LogManager.CreateLogSource(nameof(TaskWatcher<T>));

		private readonly T m_defaultResult;

		private CancellationTokenSource m_tokenSource;
		private Func<TaskStateController, Task<T>> m_doTask;
		private TaskGroup m_group;
	}
}
