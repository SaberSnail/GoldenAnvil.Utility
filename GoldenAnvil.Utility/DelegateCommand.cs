using System;
using System.Windows.Input;

namespace GoldenAnvil.Utility
{
	public sealed class DelegateCommand<T> : ICommand
	{
		public DelegateCommand(Action<T> execute)
			: this(execute, null)
		{
		}

		public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
		{
			m_execute = execute;
			m_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return m_canExecute == null || m_canExecute((T) parameter);
		}

		public void Execute(object parameter)
		{
			m_execute((T) parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}

		readonly Predicate<T> m_canExecute;
		readonly Action<T> m_execute;
	}

	public sealed class DelegateCommand : ICommand
	{
		public DelegateCommand(Action execute)
			: this(execute, null)
		{
		}

		public DelegateCommand(Action execute, Func<bool> canExecute)
		{
			m_execute = execute;
			m_canExecute = canExecute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return m_canExecute == null || m_canExecute();
		}

		public void Execute(object parameter)
		{
			m_execute();
		}

		public void RaiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
				CanExecuteChanged(this, EventArgs.Empty);
		}

		readonly Func<bool> m_canExecute;
		readonly Action m_execute;
	}
}
