using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rpc;

namespace NFive.Server.Communications
{
	public class CommunicationTarget : ICommunicationTarget
	{
		public IEventManager EventManager { get; }

		public string Event { get; }

		public CommunicationTarget(IEventManager eventManager, string @event)
		{
			this.EventManager = eventManager;
			this.Event = @event;
		}

		public ICommunicationTransmitClient ToClient(IClient client) => new CommunicationTransmitClient(this, client);

		public ICommunicationReceiveClient FromClient(IClient client) => new CommunicationReceiveClient(this, client);

		public ICommunicationTransmitClient ToClients() => new CommunicationTransmitClient(this);

		public ICommunicationReceiveClient FromClients() => new CommunicationReceiveClient(this);

		public ICommunicationTransmitServer ToServer() => new CommunicationTransmitServer(this);

		public ICommunicationReceiveServer FromServer() => new CommunicationReceiveServer(this);
	}
}
