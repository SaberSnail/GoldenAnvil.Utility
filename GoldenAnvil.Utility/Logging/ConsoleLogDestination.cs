using System;

namespace GoldenAnvil.Utility.Logging
{
	public sealed class ConsoleLogDestination : ILogDestination
	{
		public ConsoleLogDestination(bool includeTimestamp)
		{
			m_includeTimestamp = includeTimestamp;
		}

		public void LogMessage(LogSeverity severity, string source, string message)
		{
			var formattedMessage = m_includeTimestamp ?
				$"{GetCurrentTimestampString()} - {SeverityToString(severity)} - {source} - {message}" :
				$"{SeverityToString(severity)} - {source} - {message}";
			Console.WriteLine(formattedMessage);
		}

		private static string GetCurrentTimestampString()
		{
			var timestamp = DateTime.Now;
			return $"{timestamp.ToShortDateString()} {timestamp.ToLongTimeString()}";
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

		readonly bool m_includeTimestamp;
	}
}
