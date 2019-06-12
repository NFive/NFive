using System.Collections.Generic;
using NFive.SDK.Core.Diagnostics;
using JetBrains.Annotations;
using NFive.SDK.Core.Controllers;

namespace NFive.Server.Configuration
{
	[PublicAPI]
	public class CoreConfiguration : ControllerConfiguration
	{
		public override string FileName => "nfive";

		public DisplayConfiguration Display { get; set; } = new DisplayConfiguration();

		public LogConfiguration Log { get; set; } = new LogConfiguration();

		[PublicAPI]
		public class DisplayConfiguration
		{
			public string Name { get; set; } = "NFive";

			public string Game { get; set; } = "Custom";

			public string Map { get; set; } = "Los Santos";
		}

		public class LogConfiguration
		{
			public LogOutputConfiguration Output { get; set; } = new LogOutputConfiguration();

			public LogLevel Core { get; set; } = LogLevel.Info;

			public LogLevel Rpc { get; set; } = LogLevel.Info;

			public LogLevel Events { get; set; } = LogLevel.Info;

			public Dictionary<string, LogLevel> Plugins { get; set; } = new Dictionary<string, LogLevel>
			{
				{ "default", LogLevel.Info }
			};
		}

		public class LogOutputConfiguration
		{
			public LogLevel ClientConsole { get; set; } = LogLevel.Warn;

			public LogLevel ClientMirror { get; set; } = LogLevel.Warn;

			public LogLevel ServerConsole { get; set; } = LogLevel.Warn;
		}
	}
}
