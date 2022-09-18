using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Storage;
using NFive.Server.Configuration;
using NFive.Server.Models;
using NFive.Server.Storage;
using ILogger = NFive.SDK.Core.Diagnostics.ILogger;
using ServerConfiguration = NFive.SDK.Server.Configuration.ServerConfiguration;

namespace NFive.Server.Controllers
{
	public class DatabaseController : ConfigurableController<DatabaseConfiguration>
	{
		private readonly BootHistory currentBoot;

		public DatabaseController(ILogger logger, DatabaseConfiguration configuration, ICommunicationManager comms) : base(logger, configuration)
		{
			// Set global database options
			ServerConfiguration.DatabaseConnection = this.Configuration.Connection.ToString();
			ServerConfiguration.AutomaticMigrations = this.Configuration.Migrations.Automatic;

			// Enable SQL query logging
			var optionsBuilder = new DbContextOptionsBuilder<EFContext<StorageContext>>();
			optionsBuilder.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));

			BootHistory lastBoot;

			using (var context = new StorageContext())
			{
				// Create database if needed
				if (!((RelationalDatabaseCreator)context.GetService<IDatabaseCreator>()).Exists())
				{
					this.Logger.Info($"No existing database found, creating new database \"{this.Configuration.Connection.Database}\"");
				}

				if (context.Database.GetPendingMigrations().Any())
				{
					context.Database.Migrate();

					foreach (var migration in context.Database.GetAppliedMigrations())
					{
						this.Logger.Debug($"Applied migration: {migration}");
					}
				}

				lastBoot = context.BootHistory.OrderByDescending(b => b.Created).FirstOrDefault() ?? new BootHistory();

				this.currentBoot = new BootHistory();
				context.BootHistory.Add(this.currentBoot);
				context.SaveChanges();
			}

			Task.Factory.StartNew(UpdateBootHistory);

			comms.Event(BootEvents.GetTime).FromServer().On(e => e.Reply(this.currentBoot.Created));
			comms.Event(BootEvents.GetLastTime).FromServer().On(e => e.Reply(lastBoot.Created));
			comms.Event(BootEvents.GetLastActiveTime).FromServer().On(e => e.Reply(lastBoot.LastActive));
		}

		private async Task UpdateBootHistory()
		{
			while (true)
			{
				try
				{
					using (var context = new StorageContext())
					using (var transaction = await context.Database.BeginTransactionAsync())
					{
						this.currentBoot.LastActive = DateTime.UtcNow;
						context.BootHistory.Update(this.currentBoot);
						await context.SaveChangesAsync();
						await transaction.CommitAsync();
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
