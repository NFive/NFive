using CitizenFX.Core;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Rpc;
using NFive.SDK.Server.Rpc;
using NFive.Server.Diagnostics;

namespace NFive.Server.Rpc
{
	public static class RpcManager
	{
		private static Logger logger;
		private static Serializer serializer;
		private static RpcTrigger trigger;
		private static ClientHandler handler;

		public static void Configure(LogLevel level, EventHandlerDictionary events)
		{
			logger = new Logger(level, "RPC");
			serializer = new Serializer();
			trigger = new RpcTrigger(logger, serializer);
			handler = new ClientHandler(events);
		}

		public static IRpc Event(string @event) => new Rpc(@event, logger, handler, trigger, serializer);
	}
}
