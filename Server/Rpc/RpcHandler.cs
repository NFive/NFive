using NFive.SDK.Server.Rpc;

namespace NFive.Server.Rpc
{
	public class RpcHandler : IRpcHandler
	{
		public IRpc Event(string @event) => RpcManager.Event(@event);
	}
}
