using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rpc;
using System;
using System.Threading.Tasks;
using NFive.SDK.Core.IoC;
using NFive.SDK.Server.Communications;
using NFive.Server.Events;
using NFive.Server.Rpc;

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
