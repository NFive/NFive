using System;
using JetBrains.Annotations;
using NFive.SDK.Core.Controllers;

namespace NFive.Server.Configuration
{
	[PublicAPI]
	public class DatabaseConfiguration : ControllerConfiguration
	{
		public override string FileName => "database";

		public DatabaseConnectionConfiguration Connection { get; set; } = new DatabaseConnectionConfiguration();

		public DatabaseMigrationsConfiguration Migrations { get; set; } = new DatabaseMigrationsConfiguration();

		public DatabaseBootHistoryConfiguration BootHistory { get; set; } = new DatabaseBootHistoryConfiguration();

		[PublicAPI]
		public class DatabaseConnectionConfiguration
		{
			public string Host { get; set; } = "localhost";

			public int Port { get; set; } = 3306;

			public string Database { get; set; } = "fivem";

			public string User { get; set; } = "root";

			public string Password { get; set; } = string.Empty;

			public string Charset { get; set; } = "utf8mb4";

			// ReSharper disable once RedundantDefaultMemberInitializer
			public bool Logging { get; set; } = false;

			public override string ToString() => $"Host={this.Host};Port={this.Port};Database={this.Database};User Id={this.User};Password={this.Password};CharSet={this.Charset};SSL Mode=None;AllowPublicKeyRetrieval=true;Logging={this.Logging}";
		}

		[PublicAPI]
		public class DatabaseMigrationsConfiguration
		{
			public bool Automatic { get; set; } = true;
		}

		[PublicAPI]
		public class DatabaseBootHistoryConfiguration
		{
			private TimeSpan updateFrequency = TimeSpan.FromSeconds(15);

			public TimeSpan UpdateFrequency
			{
				get => this.updateFrequency;
				set => this.updateFrequency = TimeSpan.FromSeconds(Math.Max(value.TotalSeconds, 10));
			}
		}
	}
}
