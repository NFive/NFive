using CitizenFX.Core;
using NFive.Client.Diagnostics;
using NFive.SDK.Client.Rpc;
using NFive.SDK.Core.Rpc;

namespace NFive.Client.Rpc
{
	public static class RpcManager
	{
		private static readonly Logger Logger;
		private static readonly Serializer Serializer;
		private static readonly RpcTrigger Trigger;
		private static ServerHandler handler;

		static RpcManager()
		{
			Logger = new Logger("RPC");
			Serializer = new Serializer();
			Trigger = new RpcTrigger(Logger);
		}

		public static void Configure(EventHandlerDictionary events)
		{
			handler = new ServerHandler(events);
		}

		public static IRpc Event(string @event) => new Rpc(@event, Logger, handler, Trigger, Serializer);
	}
}
