﻿namespace GoldenAnvil.Utility.Logging
{
	public interface ILogDestination
	{
		void LogMessage(LogSeverity severity, string source, string message);
	}
}
