using System;
using NFive.SDK.Client.Configuration;
using NFive.SDK.Core.Diagnostics;

namespace NFive.Client.Diagnostics
{
	public class Logger : ILogger
	{
		public string Prefix { get; }

		public Logger(string prefix = "")
		{
			this.Prefix = prefix;
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

		public void Log(string message, LogLevel level)
		{
			if (ClientConfiguration.LogLevel > level) return;

			var output = $"{DateTime.Now:s} [{level}]";

			if (!string.IsNullOrEmpty(this.Prefix)) output += $" [{this.Prefix}]";

			CitizenFX.Core.Debug.Write($"{output} {message}{Environment.NewLine}");
		}
	}
}
