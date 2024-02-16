using Autofac;
using Autofac.Core;
using Microsoft.Extensions.Logging;
using NFive.SDK.Plugins.Configuration;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Configuration;
using NFive.SDK.Server.Rcon;
using NFive.Server.Communications;
using NFive.Server.Configuration;
using NFive.Server.Controllers;
using NFive.Server.Diagnostics;
using NFive.Server.Events;
using NFive.Server.Rcon;
using NFive.Server.Storage;

namespace NFive.Server.IoC
{
	public class CoreModule : Module
	{
		protected CoreConfiguration Configuration { get; set; }

		protected DatabaseConfiguration DatabaseConfiguration { get; set; }

		public CoreModule(CoreConfiguration configuration, DatabaseConfiguration databaseConfiguration)
		{
			this.Configuration = configuration;
			this.DatabaseConfiguration = databaseConfiguration;

			ServerConfiguration.DatabaseConnection = this.DatabaseConfiguration.Connection.ToString();

			StorageContext.EnableLogging = this.DatabaseConfiguration.Connection.Logging;
		}

		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(l => new Logger(this.Configuration.Log.Core))
				.As<Logger>()
				.AsImplementedInterfaces();

			builder.Register(e => new EventManager(this.Configuration.Log.Comms))
				.As<EventManager>()
				.SingleInstance();

			builder.RegisterType<StorageContext>()
				.AsSelf()
				.WithParameter((p, ctx) => p.ParameterType == typeof(string) && p.Name == "connectionString", (p, ctx) => this.DatabaseConfiguration.Connection)
				.InstancePerDependency();

			builder.RegisterType<CommunicationManager>()
				.As<ICommunicationManager>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<ClientList>()
				.WithParameter((p, ctx) => p.ParameterType == typeof(ILogger) && p.Name == "logger", (p, ctx) => new Logger(this.Configuration.Log.Core, "ClientList"))
				.As<IClientList>()
				.SingleInstance();

			builder.RegisterType<RconManager>()
				.As<IRconManager>()
				.AsSelf()
				.SingleInstance();

			builder.RegisterType<DatabaseController>()
				.AsSelf()
				.SingleInstance()
				.WithParameters(new[]
			{
				new ResolvedParameter((p, ctx) => p.ParameterType == typeof(ILogger) && p.Name == "logger", (p, ctx) => new Logger(this.Configuration.Log.Core, "Database")),
				new ResolvedParameter((p, ctx) => p.ParameterType == typeof(DatabaseConfiguration) && p.Name == "configuration", (p, ctx) => ConfigurationManager.Load<DatabaseConfiguration>("database.yml"))
			});

			builder.RegisterType<EventController>()
				.AsSelf()
				.SingleInstance()
				.WithParameter(new ResolvedParameter((p, ctx) => p.ParameterType == typeof(ILogger) && p.Name == "logger", (p, ctx) => new Logger(this.Configuration.Log.Core, "FiveM")));

			builder.RegisterType<SessionController>()
				.AsSelf()
				.SingleInstance()
				.WithParameters(new[]
			{
				new ResolvedParameter((p, ctx) => p.ParameterType == typeof(ILogger) && p.Name == "logger", (p, ctx) => new Logger(this.Configuration.Log.Core, "Session")),
				new ResolvedParameter((p, ctx) => p.ParameterType == typeof(SessionConfiguration) && p.Name == "configuration", (p, ctx) => ConfigurationManager.Load<SessionConfiguration>("session.yml"))
			});
		}
	}
}
