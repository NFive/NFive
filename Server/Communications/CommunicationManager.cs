using Griffin.Container;
using NFive.SDK.Server.Communications;
using NFive.Server.Events;

namespace NFive.Server.Communications
{
	[Component(Lifetime = Lifetime.Singleton)]
	public class CommunicationManager : ICommunicationManager
	{
		private readonly EventManager eventManager;

		public CommunicationManager(EventManager eventManager)
		{
			this.eventManager = eventManager;
		}

		public ICommunicationTarget Event(string @event) => new CommunicationTarget(this.eventManager, @event);
	}
}
