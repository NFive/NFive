using System;
using CitizenFX.Core;

namespace NFive.Server.Rpc
{
	public class ClientHandler
	{
		private readonly EventHandlerDictionary events;

		public ClientHandler(EventHandlerDictionary events)
		{
			this.events = events;
		}

		public void Attach(string @event, Delegate callback)
		{
			this.events[@event] += callback;
		}

		public void Detach(string @event, Delegate callback)
		{
			this.events[@event] -= callback;
		}
	}
}
