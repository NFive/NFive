using NFive.Client.Rpc;
using NFive.SDK.Client.Events;
using System;

namespace NFive.Client.Communications
{
	public class CommunicationMessage : ICommunicationMessage
	{
		public Guid Id { get; } = Guid.NewGuid();

		public string Event { get; }

		public CommunicationMessage(string @event)
		{
			this.Event = @event;
		}

		public void Reply(params object[] payloads)
		{
			RpcManager.Emit(this.Id + ":" + this.Event, payloads);
		}
	}
}
