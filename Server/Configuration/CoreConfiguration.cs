using System.Collections.Generic;
using System.Globalization;
using JetBrains.Annotations;
using NFive.SDK.Core.Configuration;
using NFive.SDK.Core.Controllers;
using NFive.SDK.Core.Diagnostics;
using TimeZoneConverter;

namespace NFive.Server.Configuration
{
	[PublicAPI]
	public class CoreConfiguration : ControllerConfiguration
	{
		public override string FileName => "core";

		public DisplayConfiguration Display { get; set; } = new DisplayConfiguration();

		public LocaleConfiguration Locale { get; set; } = new LocaleConfiguration
		{
			Culture = new List<CultureInfo>
			{
				new CultureInfo("en-US")
			},
			TimeZone = TZConvert.GetTimeZoneInfo("Pacific Standard Time")
		};

		public LogConfiguration Log { get; set; } = new LogConfiguration();

		[PublicAPI]
		public class DisplayConfiguration
		{
			public string Name { get; set; } = "NFive";

			public string Game { get; set; } = "Custom";

			public string Map { get; set; } = "Los Santos";
		}

		[PublicAPI]
		public class LogConfiguration
		{
			public LogOutputConfiguration Output { get; set; } = new LogOutputConfiguration();

			public LogLevel Core { get; set; } = LogLevel.Info;

			public LogLevel Comms { get; set; } = LogLevel.Info;

			public Dictionary<string, LogLevel> Plugins { get; set; } = new Dictionary<string, LogLevel>
			{
				{ "default", LogLevel.Info }
			};
		}

		[PublicAPI]
		public class LogOutputConfiguration
		{
			public LogLevel ClientConsole { get; set; } = LogLevel.Warn;

			public LogLevel ClientMirror { get; set; } = LogLevel.Warn;

			public LogLevel ServerConsole { get; set; } = LogLevel.Warn;
		}
	}
}
