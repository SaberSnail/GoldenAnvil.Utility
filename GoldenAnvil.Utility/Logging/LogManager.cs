using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GoldenAnvil.Utility.Logging
{
	public sealed class LogManager
	{
		public static ILogSource CreateLogSource(string name)
		{
			return new LogSource(name);
		}

		public static void Initialize(params ILogDestination[] destinations)
		{
			if (s_instance != s_nullManager)
				throw new InvalidOperationException($"{nameof(LogManager)} has already been initialized.");
			s_instance = new LogManager(destinations);
		}

		internal static LogManager Instance => s_instance;

		internal void LogMessage(LogSeverity severity, string message)
		{
			foreach (var destination in m_destinations)
				destination.LogMessage(severity, message);
		}

		private LogManager(IEnumerable<ILogDestination> destinations)
		{
			m_destinations = destinations.EmptyIfNull().ToList().AsReadOnly();
		}

		static readonly LogManager s_nullManager = new LogManager(null);

		static LogManager s_instance = s_nullManager;

		readonly ReadOnlyCollection<ILogDestination> m_destinations;
	}
}
