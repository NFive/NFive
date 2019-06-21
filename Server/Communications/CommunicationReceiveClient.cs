using System;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Rpc;
using NFive.Server.Rpc;

namespace NFive.Server.Communications
{
	public class CommunicationReceiveClient : ICommunicationReceiveClient
	{
		private readonly IClient targetClient;
		private readonly CommunicationTarget target;
		private readonly IRpc rpc;

		public CommunicationReceiveClient(CommunicationTarget target)
		{
			this.target = target;
			this.rpc = RpcManager.Event(target.Event);
		}

		public CommunicationReceiveClient(CommunicationTarget target, IClient client) : this(target)
		{
			this.targetClient = client;
		}

		public void On(Action<IRpcEvent> callback)
		{
			this.rpc.On(callback);
		}

		public void On<T>(Action<IRpcEvent, T> callback)
		{
			this.rpc.On(callback);
		}

		public void On<T1, T2>(Action<IRpcEvent, T1, T2> callback)
		{
			this.rpc.On(callback);
		}

		public void On<T1, T2, T3>(Action<IRpcEvent, T1, T2, T3> callback)
		{
			this.rpc.On(callback);
		}

		public void On<T1, T2, T3, T4>(Action<IRpcEvent, T1, T2, T3, T4> callback)
		{
			this.rpc.On(callback);
		}

		public void On<T1, T2, T3, T4, T5>(Action<IRpcEvent, T1, T2, T3, T4, T5> callback)
		{
			this.rpc.On(callback);
		}
	}
}
