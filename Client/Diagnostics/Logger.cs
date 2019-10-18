using System;
using System.Linq;
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
			Error(exception, "ERROR");
		}

		public void Error(Exception exception, string message)
		{
			Log($"{message}: {exception.Message}", LogLevel.Error); // TODO: Output more details
		}

		public void Log(string message, LogLevel level)
		{
			if (ClientConfiguration.Log.ConsoleLogLevel > level && ClientConfiguration.Log.MirrorLogLevel > level) return;

			var output = $"{DateTime.Now:s} [{level}]";

			if (!string.IsNullOrEmpty(this.Prefix)) output += $" [{this.Prefix}]";

			var lines = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var formattedMessage = string.Join(Environment.NewLine, lines.Select(l => $"{output} {l}"));

			if (ClientConfiguration.Log.ConsoleLogLevel <= level)
			{
				CitizenFX.Core.Debug.Write($"{formattedMessage}{Environment.NewLine}");
			}

			if (ClientConfiguration.Log.MirrorLogLevel <= level)
			{
				//RpcManager.Emit(CoreEvents.LogMirror, DateTime.UtcNow, level, this.Prefix, message); // TODO: Event const
			}
		}
	}
}
