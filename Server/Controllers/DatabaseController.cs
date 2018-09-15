using MySql.Data.MySqlClient;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Server.Configuration;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rpc;
using NFive.Server.Configuration;
using NFive.Server.Models;
using NFive.Server.Storage;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;

namespace NFive.Server.Controllers
{
	public class DatabaseController : ConfigurableController<DatabaseConfiguration>
	{
		public DatabaseController(ILogger logger, IEventManager events, IRpcHandler rpc, DatabaseConfiguration configuration) : base(logger, events, rpc, configuration)
		{
			// Set global database options
			ServerConfiguration.DatabaseConnection = this.Configuration.Connection.ToString();
			ServerConfiguration.AutomaticMigrations = this.Configuration.Migrations.Automatic;

			// Enable SQL query logging
			MySqlTrace.Switch.Level = SourceLevels.All;
			MySqlTrace.Listeners.Add(new ConsoleTraceListener());

			using (var context = new StorageContext())
			{
				// Create database if needed
				if (!context.Database.Exists())
				{
					this.Logger.Info($"No existing database found, creating new database \"{this.Configuration.Connection.Database}\"");

					context.Database.CreateIfNotExists();
				}

				context.BootHistory.Add(new BootHistory());
				context.SaveChanges();
			}

			Task.Factory.StartNew(UpdateBootHistory);
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
						context.BootHistory.OrderByDescending(b => b.Created).First().LastActive = DateTime.UtcNow;
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
					await BaseScript.Delay(this.Configuration.BootHistoryFrequency);
				}
			}
		}
	}
}
