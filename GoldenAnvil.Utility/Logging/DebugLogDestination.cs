using System;
using System.Diagnostics;

namespace GoldenAnvil.Utility.Logging
{
	public sealed class DebugLogDestination : ILogDestination
	{
		public DebugLogDestination()
		{
		}

		public void LogMessage(LogSeverity severity, string source, string message)
		{
			var formattedMessage = $"{SeverityToString(severity)} - {source} - {message}";
			Debug.WriteLine(formattedMessage);
		}

		private static string SeverityToString(LogSeverity severity)
		{
			switch (severity)
			{
			case LogSeverity.Info:
				return "INFO";
			case LogSeverity.Warn:
				return "WARN";
			case LogSeverity.Error:
				return "ERROR";
			default:
				throw new NotImplementedException($"{nameof(SeverityToString)} is not implemented for '{severity}'.");
			}
		}
	}
}
