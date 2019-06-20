using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Core.Plugins;
using System.Collections.Generic;

namespace NFive.Server.Configuration
{
	[PublicAPI]
	public class ClientConfiguration
	{
		public User User { get; set; }

		public List<Plugin> Plugins { get; set; } = new List<Plugin>();

		public LogConfiguration Log { get; set; } = new LogConfiguration();

		[PublicAPI]
		public class LogConfiguration
		{
			public bool Mirror { get; set; } = true;

			public LogLevel Core { get; set; } = LogLevel.Info;

			public LogLevel Rpc { get; set; } = LogLevel.Info;

			public LogLevel Events { get; set; } = LogLevel.Info;

			public Dictionary<string, LogLevel> Plugins { get; set; } = new Dictionary<string, LogLevel>
			{
				{ "default", LogLevel.Info }
			};
		}
	}
}
