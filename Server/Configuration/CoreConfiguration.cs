using System;
using System.Collections.Generic;
using System.Globalization;
using NFive.SDK.Core.Diagnostics;
using JetBrains.Annotations;
using NFive.SDK.Core.Controllers;
using NFive.SDK.Server.Configuration;

namespace NFive.Server.Configuration
{
	[PublicAPI]
	public class CoreConfiguration : ControllerConfiguration
	{
		public override string FileName => "core";

		public DisplayConfiguration Display { get; set; } = new DisplayConfiguration();

		public LogConfiguration Log { get; set; } = new LogConfiguration();

		public LocaleConfiguration Locale { get; set; } = new LocaleConfiguration();

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

		public class LocaleConfiguration : ILocaleConfiguration
		{
			public CultureInfo Culture { get; set; } = new CultureInfo("en-US");

			public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
		}
	}
}
