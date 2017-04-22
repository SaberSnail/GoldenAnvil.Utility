using System;
using System.Diagnostics;

namespace GoldenAnvil.Utility.Logging
{
	public static class LogSourceUtility
	{
		public static Scope TimedInfo(this ILogSource logger, string message)
		{
			return CreateTimedLogScope(logger.Info, message);
		}

		public static Scope TimedWarn(this ILogSource logger, string message)
		{
			return CreateTimedLogScope(logger.Warn, message);
		}

		public static Scope TimedError(this ILogSource logger, string message)
		{
			return CreateTimedLogScope(logger.Error, message);
		}

		private static Scope CreateTimedLogScope(Action<string> doLogMessage, string message)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			doLogMessage($"(start) {message}");
			return Scope.Create(() =>
			{
				TimeSpan elapsed = stopwatch.Elapsed;
				doLogMessage($"({elapsed}) {message}");
			});
		}
	}
}
