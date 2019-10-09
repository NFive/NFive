using System;
using System.Collections.Generic;
using CitizenFX.Core;
using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.IoC;
using NFive.SDK.Core.Rpc;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Events;

namespace NFive.Server
{
	[PublicAPI]
	[Component(Lifetime = Lifetime.Singleton)]
	public class ClientList : IClientList
	{
		private readonly ILogger logger;

		public List<IClient> Clients { get; } = new List<IClient>();

		public event EventHandler<ClientEventArgs> ClientAdded;

		public event EventHandler<ClientEventArgs> ClientRemoved;

		public ClientList(ILogger logger, ICommunicationManager comms)
		{
			this.logger = logger;

			comms.Event(RpcEvents.ClientInitialize).FromClients().On(OnInitialize);
			comms.Event("nfive:server:playerDropped").FromServer().On<IClient, string, CallbackDelegate>(OnDropped);
		}

		private void OnInitialize(ICommunicationMessage e)
		{
			this.logger.Trace($"Client added: {e.Client.Name} [{e.Client.Handle}]");

			this.Clients.Add(e.Client);

			this.ClientAdded?.Invoke(this, new ClientEventArgs(e.Client));
		}

		private void OnDropped(ICommunicationMessage e, IClient client, string disconnectMessage, CallbackDelegate drop)
		{
			this.logger.Trace($"Client disconnected: {client.Name}");

			this.Clients.RemoveAll(c => c.Handle == client.Handle);

			this.ClientRemoved?.Invoke(this, new ClientEventArgs(e.Client));
		}
	}
}
