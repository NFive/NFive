using System;
using NFive.SDK.Core.Diagnostics;

namespace NFive.Server.Diagnostics
{
	public class Logger : ILogger
	{
		public LogLevel Level { get; }

		public string Prefix { get; }

		public Logger(LogLevel minLevel = LogLevel.Info, string prefix = "")
		{
			this.Level = minLevel;
			this.Prefix = prefix;
		}

		public void Trace(string message)
		{
			Log(message, LogLevel.Trace);
		}

		public void Debug(string message)
		{
			Log(message, LogLevel.Debug);
		}

		public void Info(string message)
		{
			Log(message, LogLevel.Info);
		}

		public void Warn(string message)
		{
			Log(message, LogLevel.Warn);
		}

		public void Error(Exception exception)
		{
			Log($"ERROR: {exception.Message}", LogLevel.Error);
		}

		public void Error(Exception exception, string message)
		{
			Log($"{message}: {exception.Message}", LogLevel.Error);
		}

		public void Log(string message, LogLevel level)
		{
			if (this.Level > level) return;

			var output = $"{DateTime.Now:s} [{level}]";

			if (!string.IsNullOrEmpty(this.Prefix)) output += $" [{this.Prefix}]";

			CitizenFX.Core.Debug.Write($"{output} {message}{Environment.NewLine}");
		}
	}
}
