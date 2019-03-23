using MySql.Data.MySqlClient;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Server.Configuration;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rcon;
using NFive.SDK.Server.Rpc;
using NFive.Server.Configuration;
using NFive.Server.Models;
using NFive.Server.Storage;
using System;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NFive.Server.Controllers
{
	public class DatabaseController : ConfigurableController<DatabaseConfiguration>
	{
		private readonly BootHistory currentBoot;

		public DatabaseController(ILogger logger, IEventManager events, IRpcHandler rpc, IRconManager rcon, DatabaseConfiguration configuration) : base(logger, events, rpc, rcon, configuration)
		{
			// Set global database options
			ServerConfiguration.DatabaseConnection = this.Configuration.Connection.ToString();
			ServerConfiguration.AutomaticMigrations = this.Configuration.Migrations.Automatic;

			// Enable SQL query logging
			MySqlTrace.Switch.Level = SourceLevels.All;
			MySqlTrace.Listeners.Add(new ConsoleTraceListener());

			BootHistory lastBoot;

			using (var context = new StorageContext())
			{
				// Create database if needed
				if (!context.Database.Exists())
				{
					this.Logger.Info($"No existing database found, creating new database \"{this.Configuration.Connection.Database}\"");

					var migrator = new DbMigrator(new Migrations.Configuration());
					foreach (var migration in migrator.GetPendingMigrations())
					{
						this.Logger.Debug($"Running migration: {migration}");

						migrator.Update(migration);
					}
				}

				lastBoot = context.BootHistory.OrderByDescending(b => b.Created).FirstOrDefault() ?? new BootHistory();

				this.currentBoot = new BootHistory();
				context.BootHistory.Add(this.currentBoot);
				context.SaveChanges();
			}

			Task.Factory.StartNew(UpdateBootHistory);

			this.Events.OnRequest(BootEvents.GetTime, () => this.currentBoot.Created);
			this.Events.OnRequest(BootEvents.GetLastTime, () => lastBoot.Created);
			this.Events.OnRequest(BootEvents.GetLastActiveTime, () => lastBoot.LastActive);
		}

		private async Task UpdateBootHistory()
		{
			while (true)
			{
				try
				{
					using (var context = new StorageContext())
					using (var transaction = context.Database.BeginTransaction())
					{
						this.currentBoot.LastActive = DateTime.UtcNow;
						context.BootHistory.AddOrUpdate(this.currentBoot);
						await context.SaveChangesAsync();
						transaction.Commit();
					}
				}
				catch (Exception ex)
				{
					this.Logger.Error(ex);
				}
				finally
				{
					await Task.Delay(this.Configuration.BootHistory.UpdateFrequency);
				}
			}
		}
	}
}
