using System;
using System.Collections.Generic;
using CitizenFX.Core;
using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.IoC;
using NFive.SDK.Server;
using NFive.SDK.Server.Rpc;
using NFive.Server.Diagnostics;

namespace NFive.Server
{
	[PublicAPI]
	[Component(Lifetime = Lifetime.Singleton)]
	public class ClientList : IClientList
	{
		public List<IClient> Clients { get; private set; } = new List<IClient>();

		public ClientList(IRpcHandler rpc)
		{
			rpc.Event(SDK.Core.Rpc.RpcEvents.ClientInitialized).On(Connecting);
			rpc.Event("playerDropped").OnRaw(new Action<Player, string, CallbackDelegate>(Dropped));
		}

		private void Connecting(IRpcEvent e)
		{
			new Logger(LogLevel.Trace, "ClientList").Debug($"Client Initialized: {e.Client.Name} | Handle: {e.Client.Handle}");
			this.Clients.Add(e.Client);
		}

		private void Dropped([FromSource] Player player, string disconnectMessage, CallbackDelegate drop)
		{
			new Logger(LogLevel.Debug, "ClientList").Debug($"Client disconnected: {player.Name}");
			this.Clients.RemoveAll(c => c.Handle == int.Parse(player.Handle));
		}
	}
}
