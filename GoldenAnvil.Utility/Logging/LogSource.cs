namespace GoldenAnvil.Utility.Logging
{
	internal sealed class LogSource : ILogSource
	{
		public LogSource(string name)
		{
			Name = name;
		}

		private string Name { get; }

		public void Info(string message)
		{
			LogManager.Instance.LogMessage(LogSeverity.Info, Name, message);
		}

		public void Warn(string message)
		{
			LogManager.Instance.LogMessage(LogSeverity.Warn, Name, message);
		}

		public void Error(string message)
		{
			LogManager.Instance.LogMessage(LogSeverity.Error, Name, message);
		}
	}
}
