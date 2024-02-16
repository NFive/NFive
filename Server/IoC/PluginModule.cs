using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Microsoft.EntityFrameworkCore;
using NFive.SDK.Core.Controllers;
using NFive.SDK.Core.Plugins;
using NFive.SDK.Plugins.Configuration;
using NFive.SDK.Server;
using NFive.SDK.Server.Configuration;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.IoC;
using NFive.SDK.Server.Storage;
using NFive.Server.Configuration;
using NFive.Server.Diagnostics;
using NFive.Server.Extensions;
using NFive.Server.Storage;
using ILogger = NFive.SDK.Core.Diagnostics.ILogger;
using LogLevel = NFive.SDK.Core.Diagnostics.LogLevel;
using Module = Autofac.Module;

namespace NFive.Server.IoC
{
	public class PluginModule : Module
	{
		protected readonly CoreConfiguration Config;

		protected readonly DatabaseConfiguration DatabaseConfiguration;

		protected readonly List<NFive.SDK.Plugins.Plugin> Plugins;

		public PluginModule(List<NFive.SDK.Plugins.Plugin> plugins, DatabaseConfiguration databaseConfiguration)
		{
			this.Plugins = plugins;
			this.DatabaseConfiguration = databaseConfiguration;
			this.Config = ConfigurationManager.Load<CoreConfiguration>("core.yml");

			ServerConfiguration.DatabaseConnection = this.DatabaseConfiguration.Connection.ToString();

			StorageContext.EnableLogging = this.DatabaseConfiguration.Connection.Logging;
		}

		protected override void Load(ContainerBuilder builder)
		{
			foreach (var plugin in this.Plugins)
			{
				RegisterPluginIncludeFiles(builder, plugin);

				RegisterPluginMainFiles(builder, plugin);
			}
		}

		protected void RegisterPluginIncludeFiles(ContainerBuilder builder, Plugin plugin)
		{
			foreach (var asm in from includeName in plugin.Server?.Include ?? new List<string>() select plugin.GetIncludeFileAssembly(includeName)) builder.RegisterAssemblyTypes(asm).Where(t => !t.IsAbstract && t.IsClass && t.IsSubclassOf(typeof(ControllerConfiguration))).AsSelf().PreserveExistingDefaults().SingleInstance().Named(plugin.Name, typeof(ControllerConfiguration));
		}
		
		protected void RegisterPluginMainFiles(ContainerBuilder builder, Plugin plugin)
		{
			var pluginDefaultLogLevel = this.Config.Log.Plugins.TryGetValue("default", out var logLevel) ? logLevel : LogLevel.Info;

			foreach (var asm in from mainName in plugin.Server?.Main ?? new List<string>() select plugin.GetMainFileAssembly(mainName))
			{
				var sdkVersion = asm.GetCustomAttribute<ServerPluginAttribute>();

				if (sdkVersion != null)
				{
					if (sdkVersion.Target != SDK.Server.SDK.Version)
					{
						throw new Exception($"Unable to load outdated SDK plugin from plugin \"{plugin.FullName}\".");
					}

					builder.RegisterAssemblyTypes(asm)
						.AsClosedTypesOf(typeof(EFContext<>))
						.AsSelf()
						.As<DbContext>()
						.WithParameter((p, ctx) => p.ParameterType == typeof(string) && p.Name == "connectionString", (p, ctx) => this.DatabaseConfiguration.Connection)
						.PreserveExistingDefaults()
						.Named(plugin.Name, typeof(EFContext<>));

					builder.RegisterAssemblyTypes(asm)
						.Where(t => !t.IsAbstract && t.IsClass && t == typeof(Controller))
						.WithParameter(new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(ILogger) && pi.Name == "logger", (pi, ctx) => new Logger(this.Config.Log.Plugins.TryGetValue(plugin.Name, out var logPlugin) ? logPlugin : pluginDefaultLogLevel, plugin.Name)))
						.AsSelf()
						.PreserveExistingDefaults()
						.SingleInstance()
						.Named<Controller>(plugin.Name);

					builder.RegisterAssemblyTypes(asm)
						.Where(t => !t.IsAbstract && t.IsClass && t.IsSubclassOf(typeof(ControllerConfiguration)))
						.AsSelf()
						.PreserveExistingDefaults()
						.SingleInstance()
						.Named<ControllerConfiguration>(plugin.Name);

					builder.RegisterAssemblyTypes(asm)
						.AsClosedTypesOf(typeof(ConfigurableController<>))
						.AsSelf()
						.PreserveExistingDefaults()
						.WithParameters(new[]
					{
						new ResolvedParameter((pi, ctx) => pi.ParameterType == typeof(ILogger) && pi.Name == "logger", (pi, ctx) => new Logger(this.Config.Log.Plugins.TryGetValue(plugin.Name, out var logPlugin) ? logPlugin : pluginDefaultLogLevel, plugin.Name)),
						new ResolvedParameter((pi, ctx) => pi.ParameterType.BaseType == typeof(ControllerConfiguration) && pi.Name == "configuration", (pi, ctx) => ConfigurationManager.InitializeConfig(plugin.Name, pi.ParameterType))
					})
						.As<Controller>()
						.Named(plugin.Name, typeof(Controller));

					builder.RegisterAssemblyTypes(asm)
						.Where(t => t.GetCustomAttribute<ComponentAttribute>()?.Lifetime == Lifetime.Singleton)
						.SingleInstance();

					builder.RegisterAssemblyTypes(asm)
						.Where(t => t.GetCustomAttribute<ComponentAttribute>()?.Lifetime == Lifetime.Transient);
				}
				else
				{
					throw new Exception($"Unable to load outdated SDK plugin from plugin \"{plugin.FullName}\"."); // TODO
				}
			}
		}
	}
}
