using NFive.Client.Rpc;
using NFive.SDK.Client.Configuration;
using NFive.SDK.Core.Diagnostics;
using System;
using System.Linq;

namespace NFive.Client.Diagnostics
{
	public class Logger : ILogger
	{
		private RpcHandler rpc = new RpcHandler();

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
			Log($"ERROR: {exception.Message}", LogLevel.Error);
		}

		public void Error(Exception exception, string message)
		{
			Log($"{message}: {exception.Message}", LogLevel.Error);
		}

		public void Log(string message, LogLevel level)
		{
			if (ClientConfiguration.LogLevel > level) return;

			var output = $"{DateTime.Now:s} [{level}]";

			if (!string.IsNullOrEmpty(this.Prefix)) output += $" [{this.Prefix}]";

			var lines = message.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			var formattedMessage = string.Join(Environment.NewLine, lines.Select(l => $"{output} {l}"));

			if (true) // TODO: Config "mirror logs only"
			{
				CitizenFX.Core.Debug.Write($"{formattedMessage}{Environment.NewLine}");
			}

			if (true) // TODO: Config "mirror logs"
			{
				this.rpc.Event("nfive:log:mirror").Trigger(DateTime.UtcNow, level, this.Prefix, message);
			}
		}
	}
}
