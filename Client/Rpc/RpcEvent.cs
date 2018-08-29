using NFive.SDK.Client.Rpc;

namespace NFive.Client.Rpc
{
	public class RpcEvent : IRpcEvent
	{
		public string Event { get; set; }

		public void Reply(params object[] payloads)
		{
			// TODO
		}
	}
}
