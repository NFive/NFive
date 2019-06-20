using JetBrains.Annotations;

namespace NFive.Server.Configuration
{
	[PublicAPI]
	public static class ServerLogConfiguration
	{
		public static CoreConfiguration.LogOutputConfiguration Output { get; set; }
	}
}
