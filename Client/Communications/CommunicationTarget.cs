using NFive.SDK.Client.Events;

namespace NFive.Client.Communications
{
	public interface IClient
	{
		int Handle { get; }

		string Name { get; }

		string EndPoint { get; }

		string License { get; }

		long? SteamId { get; }

		ulong? DiscordId { get; }

		int Ping { get; }
	}

	public interface ICommunicationTarget
	{
		IEventManager EventManager { get; }

		string Event { get; }

		ICommunicationTransmitClient ToClient(IClient client);

		ICommunicationReceiveClient FromClient(IClient client);

		ICommunicationTransmitClient ToClients();

		ICommunicationReceiveClient FromClients();

		ICommunicationTransmitServer ToServer();

		ICommunicationReceiveServer FromServer();
	}

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
