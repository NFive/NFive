using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
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

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<User>().HasIndex(u => u.License).IsUnique();
		}
	}
}
