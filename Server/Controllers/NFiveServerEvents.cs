using JetBrains.Annotations;

namespace NFive.Server.Controllers
{
	[PublicAPI]
	public static class NFiveServerEvents
	{
		public const string HostingSession = "nfive:server:hostingSession";
		public const string HostedSession = "nfive:server:hostedSession";
		public const string PlayerConnecting = "nfive:server:playerConnecting";
		public const string PlayerDropped = "nfive:server:playerDropped";
		public const string ResourceStart = "nfive:server:resourceStart";
		public const string ResourceStop = "nfive:server:resourceStop";
		public const string RrconCommand = "nfive:server:rconCommand";
		public const string GameEventTriggered = "nfive:server:gameEventTriggered";
	}
}
