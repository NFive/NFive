using JetBrains.Annotations;

namespace NFive.Server.Controllers
{
	[PublicAPI]
	public static class FiveMServerEvents
	{
		public const string HostingSession = "hostingSession";
		public const string HostedSession = "HostedSession";
		public const string PlayerConnecting = "playerConnecting";
		public const string PlayerDropped = "playerDropped";
		public const string ResourceStart = "onResourceStart";
		public const string ResourceStop = "onResourceStop";
		public const string RconCommand = "rconCommand";
		public const string ExplosionEvent = "explosionEvent";
	}
}
