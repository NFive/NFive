using CitizenFX.Core;
using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.IoC;
using NFive.SDK.Server;
using NFive.SDK.Server.Rpc;
using System;
using System.Collections.Generic;

namespace NFive.Server
{
	[PublicAPI]
	[Component(Lifetime = Lifetime.Singleton)]
	public class ClientList : IClientList
	{
		private readonly ILogger logger;

		public List<IClient> Clients { get; private set; } = new List<IClient>();

		public ClientList(ILogger logger, IRpcHandler rpc)
		{
			this.logger = logger;

			rpc.Event(SDK.Core.Rpc.RpcEvents.ClientInitialize).On(OnInitialize);
			rpc.Event("playerDropped").OnRaw(new Action<Player, string, CallbackDelegate>(OnDropped));
		}

		private void OnInitialize(IRpcEvent e)
		{
			this.logger.Trace($"Client added: {e.Client.Name} [{e.Client.Handle}]");
			this.Clients.Add(e.Client);
		}

		private void OnDropped([FromSource] Player player, string disconnectMessage, CallbackDelegate drop)
		{
			this.logger.Trace($"Client disconnected: {player.Name}");
			this.Clients.RemoveAll(c => c.Handle == int.Parse(player.Handle));
		}
	}
}
