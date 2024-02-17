using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Server.Storage;
using NFive.Server.Models;

namespace NFive.Server.Storage
{
	[PublicAPI]
	public class StorageContext : EFContext<StorageContext>
	{
		public DbSet<User> Users { get; set; }

		public DbSet<Session> Sessions { get; set; }

		public DbSet<BootHistory> BootHistory { get; set; }

		private ILoggerFactory loggerFactory = LoggerFactory.Create(builder => builder.AddSimpleConsole(options =>
		{
			options.IncludeScopes = true;
			options.SingleLine = true;
			options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss";
		}));

		public static bool EnableLogging { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>().HasIndex(u => u.License).IsUnique();
			modelBuilder.Entity<Session>().Ignore(s => s.Handle);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (EnableLogging)
			{
				optionsBuilder.UseLoggerFactory(this.loggerFactory);

				optionsBuilder.ConfigureWarnings(warnings =>
				{
					warnings.Default(WarningBehavior.Ignore);
					warnings.Log(RelationalEventId.TransactionCommitted, RelationalEventId.CommandExecuted);
				});
			}

			base.OnConfiguring(optionsBuilder);
		}
	}
}
