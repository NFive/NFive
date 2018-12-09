using JetBrains.Annotations;
using NFive.SDK.Server.Migrations;
using NFive.Server.Storage;

namespace NFive.Server.Migrations
{
	[PublicAPI]
	internal sealed class Configuration : MigrationConfiguration<StorageContext> { }
}
