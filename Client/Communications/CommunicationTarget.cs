using NFive.Client.Events;
using NFive.SDK.Client.Communications;

namespace NFive.Client.Communications
{
	public class CommunicationTarget : ICommunicationTarget
	{
		public EventManager EventManager { get; }

		public string Event { get; }

		public CommunicationTarget(EventManager eventManager, string @event)
		{
			this.EventManager = eventManager;
			this.Event = @event;
		}

		public ICommunicationTransmitClient ToClient() => new CommunicationTransmitClient(this.Event, this.EventManager);

		public ICommunicationReceiveClient FromClient() => new CommunicationReceiveClient(this.Event, this.EventManager);

		public ICommunicationTransmitServer ToServer() => new CommunicationTransmitServer(this);

		public ICommunicationReceiveServer FromServer() => new CommunicationReceiveServer(this);
	}
}
