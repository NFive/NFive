using System;
using CitizenFX.Core;
using JetBrains.Annotations;

namespace NFive.Server.Rpc
{
	[PublicAPI]
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
