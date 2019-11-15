using System;
using System.Collections.Generic;
using Griffin.Container;
using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Models.Player;
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

			comms.Event(SessionEvents.ClientInitialized).FromClients().On<IClient, Session>(OnInitialized);
			comms.Event(SessionEvents.ClientDisconnected).FromServer().On<IClient, Session>(OnDisconnected);
		}

		private void OnInitialized(ICommunicationMessage e, IClient client, Session session)
		{
			this.logger.Trace($"Client added: {e.Client.Name} [{e.Client.Handle}]");

			this.Clients.Add(e.Client);

			this.ClientAdded?.Invoke(this, new ClientEventArgs(e.Client));
		}

		private void OnDisconnected(ICommunicationMessage e, IClient client, Session session)
		{
			this.logger.Trace($"Client disconnected: {client.Name} [{client.Handle}]");

			this.Clients.RemoveAll(c => c.Handle == client.Handle);

			this.ClientRemoved?.Invoke(this, new ClientEventArgs(client));
		}
	}
}
