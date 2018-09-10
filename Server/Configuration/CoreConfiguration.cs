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
			public string Name { get; set; } = "NFive";

			public string Game { get; set; } = "Custom";

			public string Map { get; set; } = "Los Santos";
		}

		public class LogConfiguration
		{
			public LogLevel Level { get; set; } = LogLevel.Info;
		}
	}
}
