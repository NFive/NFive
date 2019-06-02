using NFive.SDK.Client.Rpc;

namespace NFive.Client.Rpc
{
	public class RpcEvent : IRpcEvent
	{
		public string Event { get; set; }

		public RpcEvent(string @event)
		{
			this.Event = @event;
		}

		public void Reply(params object[] payloads)
		{
			RpcManager.Event(this.Event).Trigger(payloads);
		}
	}
}
