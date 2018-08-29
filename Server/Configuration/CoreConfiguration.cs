using NFive.SDK.Core.Diagnostics;
using JetBrains.Annotations;

namespace NFive.Server.Configuration
{
	[PublicAPI]
	public class CoreConfiguration
	{
		public DisplayConfiguration Display { get; set; } = new DisplayConfiguration();

		public LogConfiguration Log { get; set; } = new LogConfiguration();

		public class DisplayConfiguration
		{
			public string Name { get; set; }

			public string Game { get; set; }

			public string Map { get; set; }
		}

		public class LogConfiguration
		{
			public LogLevel Level { get; set; } = LogLevel.Info;
		}
	}
}
