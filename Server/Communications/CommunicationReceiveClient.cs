using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Rpc;
using NFive.Server.Rpc;
using System;

namespace NFive.Server.Communications
{
	public class CommunicationReceiveClient : ICommunicationReceiveClient
	{
		private readonly IRpc rpc;
		private readonly IClient targetClient;

		public CommunicationReceiveClient(ICommunicationTarget target)
		{
			this.rpc = RpcManager.Event(target.Event);
		}

		public CommunicationReceiveClient(ICommunicationTarget target, IClient client) : this(target)
		{
			this.targetClient = client; // TODO: Specific client targeting
		}

		public void On(Action<ICommunicationMessage> callback) => this.rpc.On(callback);

		public void On<T>(Action<ICommunicationMessage, T> callback) => this.rpc.On(callback);

		public void On<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => this.rpc.On(callback);

		public void On<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => this.rpc.On(callback);

		public void On<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => this.rpc.On(callback);

		public void On<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => this.rpc.On(callback);
	}
}
