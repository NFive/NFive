using NFive.SDK.Core.Diagnostics;
using System;
using System.IO;
using System.Linq;
using NFive.Server.Configuration;

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

			var lines = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			message = string.Join(Environment.NewLine, lines.Select(l => $"{output} {l}"));

			File.AppendAllText("nfive.log", $"{message}{Environment.NewLine}");

			if (ServerLogConfiguration.Output == null || ServerLogConfiguration.Output.ServerConsole <= level) CitizenFX.Core.Debug.Write($"{message}{Environment.NewLine}");
		}
	}
}
