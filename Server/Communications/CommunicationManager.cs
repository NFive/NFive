using NFive.SDK.Core.IoC;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Events;

namespace NFive.Server.Communications
{
	[Component(Lifetime = Lifetime.Singleton)]
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
