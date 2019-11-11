using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NFive.Server.Diagnostics
{
	public static class LogWriter
	{
		private const int MaxLogFiles = 10;
		private const string FilePath = "nfive.log";

		private static readonly BlockingCollection<string> Logs = new BlockingCollection<string>();

		static LogWriter()
		{
			if (File.Exists(FilePath))
			{
				var fileRegex = new Regex($"^{Regex.Escape(FilePath)}(\\.[0-9])?$", RegexOptions.Compiled);
				var logFiles = Directory.EnumerateFiles(Environment.CurrentDirectory, $"{FilePath}.?", SearchOption.TopDirectoryOnly).Where(f => fileRegex.Match(Path.GetFileName(f) ?? f).Success).OrderBy(f => f).ToList();

				if (logFiles.Count >= MaxLogFiles)
				{
					foreach (var file in logFiles.Skip(MaxLogFiles - 1).ToList())
					{
						logFiles.Remove(file);
						File.Delete(file);
					}
				}

				for (var i = logFiles.Count - 1; i >= 0; i--)
				{
					File.Move(logFiles[i], $"{FilePath}.{i + 1}");
				}
			}

			Task.Factory.StartNew(() =>
			{
				using (var writer = File.AppendText(FilePath))
				{
					foreach (var log in Logs.GetConsumingEnumerable())
					{
						writer.Write(log);
						writer.Flush();
					}
				}
			});
		}

		public static void Add(string log)
		{
			Logs.Add(log);
		}
	}
}
