using System;
using JetBrains.Annotations;
using NFive.Client.Events;
using NFive.Client.Rpc;
using NFive.SDK.Client.Communications;

namespace NFive.Client.Communications
{
	[PublicAPI]
	public class CommunicationMessage : ICommunicationMessage
	{
		private readonly EventManager eventManager;

		private readonly bool networked;

		public Guid Id { get; } = Guid.NewGuid();

		public string Event { get; }

		public CommunicationMessage(string @event)
		{
			this.Event = @event;
		}

		public CommunicationMessage(string @event, EventManager eventManager) : this(@event)
		{
			this.eventManager = eventManager;
		}

		public CommunicationMessage(string @event, Guid id, bool networked) : this(@event)
		{
			this.Id = id;
			this.networked = networked;
		}

		public void Reply(params object[] payloads)
		{
			if (this.networked)
			{
				RpcManager.Emit($"{this.Id}:{this.Event}", payloads);
			}
			else
			{
				this.eventManager.Emit($"{this.Id}:{this.Event}", payloads);
			}
		}
	}
}
