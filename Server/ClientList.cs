using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using CitizenFX.Core;
using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.IoC;
using NFive.SDK.Server;
using NFive.SDK.Server.Rpc;
using NFive.Server.Diagnostics;
using NFive.Server.Rpc;

namespace NFive.Server
{
	[PublicAPI]
	[Component(Lifetime = Lifetime.Singleton)]
	public class ClientList : IClientList
	{
		public List<IClient> Clients { get; private set; } = new List<IClient>();

		public ClientList(IRpcHandler rpc)
		{
			rpc.Event("playerConnecting").OnRaw(new Action<Player, string, CallbackDelegate, ExpandoObject>(Connecting));
			rpc.Event("playerDropped").OnRaw(new Action<Player, string, CallbackDelegate>(Dropped));
		}

		private void Connecting([FromSource] Player player, string playerName, CallbackDelegate drop,
			ExpandoObject callbacks)
		{
			new Logger(LogLevel.Debug, "ClientList").Debug($"Client connected: {player.Name} | Handle: {player.Handle}");
			this.Clients.Add(new Client(int.Parse(player.Handle)));
		}

		private void Dropped([FromSource] Player player, string disconnectMessage, CallbackDelegate drop)
		{
			new Logger(LogLevel.Debug, "ClientList").Debug($"Client disconnected: {player.Name}");
			this.Clients.RemoveAll(c => c.Handle == int.Parse(player.Handle));
		}
	}
}
