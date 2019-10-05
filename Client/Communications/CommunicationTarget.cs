using NFive.SDK.Client.Communications;
using NFive.SDK.Client.Events;

namespace NFive.Client.Communications
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

		public ICommunicationTransmitClient ToClient() => new CommunicationTransmitClient(this);

		public ICommunicationReceiveClient FromClient() => new CommunicationReceiveClient(this);

		public ICommunicationTransmitServer ToServer() => new CommunicationTransmitServer(this);

		public ICommunicationReceiveServer FromServer() => new CommunicationReceiveServer(this);
	}
}
