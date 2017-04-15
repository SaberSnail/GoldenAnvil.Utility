namespace GoldenAnvil.Utility.Logging
{
	public interface ILogSource
	{
		void Info(string message);
		void Warn(string message);
		void Error(string message);
	}
}
