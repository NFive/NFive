using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rpc;
using NFive.Server.Events;

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

		public ICommunicationTransmitClient ToClient(IClient client)
		{
			return new CommunicationTransmitClient(this, client);
		}

		public ICommunicationReceiveClient FromClient(IClient client)
		{
			return new CommunicationReceiveClient(this, client);
		}

		public ICommunicationTransmitClient ToClients()
		{
			return new CommunicationTransmitClient(this);
		}

		public ICommunicationReceiveClient FromClients()
		{
			return new CommunicationReceiveClient(this);
		}

		public ICommunicationTransmitServer ToServer()
		{
			return new CommunicationTransmitServer(this);
		}

		public ICommunicationReceiveServer FromServer()
		{
			return new CommunicationReceiveServer(this);
		}

	}
}
