// <auto-generated />
// ReSharper disable all

using System.CodeDom.Compiler;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;

namespace NFive.Server.Migrations
{
	[GeneratedCode("NFive.Migration", "0.3 Alpha")]
	public class Init : DbMigration, IMigrationMetadata
	{
		string IMigrationMetadata.Id => "201903231848283_Init";

		string IMigrationMetadata.Source => null;

		string IMigrationMetadata.Target => "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";

		public override void Up()
		{
			CreateTable(
				"dbo.BootHistories",
				c => new
				{
					Id = c.Guid(nullable: false),
					Created = c.DateTime(nullable: false, precision: 0),
					LastActive = c.DateTime(nullable: false, precision: 0),
				})
				.PrimaryKey(t => t.Id);

			CreateTable(
				"dbo.Sessions",
				c => new
				{
					Id = c.Guid(nullable: false),
					IpAddress = c.String(nullable: false, maxLength: 47, unicode: false),
					Created = c.DateTime(nullable: false, precision: 0),
					Connected = c.DateTime(precision: 0),
					Disconnected = c.DateTime(precision: 0),
					DisconnectReason = c.String(maxLength: 200, unicode: false),
					UserId = c.Guid(nullable: false),
				})
				.PrimaryKey(t => t.Id)
				.ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
				.Index(t => t.UserId);

			CreateTable(
				"dbo.Users",
				c => new
				{
					Id = c.Guid(nullable: false),
					License = c.String(nullable: false, maxLength: 40, unicode: false),
					SteamId = c.Long(),
					Name = c.String(nullable: false, maxLength: 32, unicode: false),
					Created = c.DateTime(nullable: false, precision: 0),
					Deleted = c.DateTime(precision: 0),
				})
				.PrimaryKey(t => t.Id)
				.Index(t => t.License, unique: true)
				.Index(t => t.SteamId, unique: true);
		}

		public override void Down()
		{
			DropForeignKey("dbo.Sessions", "UserId", "dbo.Users");
			DropIndex("dbo.Users", new[] { "SteamId" });
			DropIndex("dbo.Users", new[] { "License" });
			DropIndex("dbo.Sessions", new[] { "UserId" });
			DropTable("dbo.Users");
			DropTable("dbo.Sessions");
			DropTable("dbo.BootHistories");
		}
	}
}
