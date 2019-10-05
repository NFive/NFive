using NFive.SDK.Client.Communications;
using NFive.SDK.Client.Events;

namespace NFive.Client.Communications
{

	public class CommunicationManager : ICommunicationManager
	{
		private readonly IEventManager eventManager;

		public CommunicationManager(IEventManager eventManager)
		{
			this.eventManager = eventManager;
		}

		public ICommunicationTarget Event(string @event) => new CommunicationTarget(this.eventManager, @event);
	}
}
