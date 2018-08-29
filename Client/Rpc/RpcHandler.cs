using NFive.SDK.Client.Rpc;

namespace NFive.Client.Rpc
{
	public class RpcHandler : IRpcHandler
	{
		public IRpc Event(string @event) => RpcManager.Event(@event);
	}
}
