using NFive.SDK.Core.Diagnostics;
using NFive.Server.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
			Error(exception, "ERROR");
		}

		public void Error(Exception exception, string message)
		{
			var output = $"{message}:{Environment.NewLine}{exception.Message}{Environment.NewLine}";
			var ex = exception;

			while (ex.InnerException != null)
			{
				output += $"{ex.InnerException.Message}{Environment.NewLine}";
				ex = ex.InnerException;
			}

			Log($"{output}  {exception.TargetSite} in {exception.Source}{Environment.NewLine}{exception.StackTrace}", LogLevel.Error);
		}

		public void Log(string message, LogLevel level)
		{
			if (this.Level > level) return;

			var output = $"{DateTime.Now:s} [{level}]";

			if (!string.IsNullOrEmpty(this.Prefix)) output += $" [{this.Prefix}]";

			var lines = message?.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries) ?? new[] { "" };
			message = string.Join(Environment.NewLine, lines.Select(l => $"{output} {l}"));

			File.AppendAllText("nfive.log", $"{message}{Environment.NewLine}");

			if (ServerLogConfiguration.Output == null || ServerLogConfiguration.Output.ServerConsole <= level) CitizenFX.Core.Debug.Write($"{message}{Environment.NewLine}");
		}

		public static void Initialize()
		{
			var filePath = "nfive.log";
			var maxLogs = 10;

			if (!File.Exists(filePath)) return;

			var fileRegex = new Regex($"^nfive\\.log(\\.[0-9])?$", RegexOptions.Compiled);
			var logFiles = Directory.EnumerateFiles(Environment.CurrentDirectory, $"{filePath}.?", SearchOption.TopDirectoryOnly).Where(f => fileRegex.Match(Path.GetFileName(f)).Success).OrderBy(f => f).ToList();

			if (logFiles.Count >= maxLogs)
			{
				foreach (var file in logFiles.Skip(maxLogs - 1).ToList())
				{
					logFiles.Remove(file);
					File.Delete(file);
				}
			}

			for (var i = logFiles.Count - 1; i >= 0; i--)
			{
				File.Move(logFiles[i], $"{filePath}.{i + 1}");
			}
		}
	}
}
