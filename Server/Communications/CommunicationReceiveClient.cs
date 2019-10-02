using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Rpc;
using NFive.Server.Rpc;
using System;

namespace NFive.Server.Communications
{
	public class CommunicationReceiveClient : ICommunicationReceiveClient
	{
		public string Event { get; }

		public IClient Target { get; }

		public CommunicationReceiveClient(ICommunicationTarget target)
		{
			this.Event = target.Event;
		}

		public CommunicationReceiveClient(ICommunicationTarget target, IClient client) : this(target)
		{
			this.Target = client;
		}

		public void On(Action<ICommunicationMessage> callback) => RpcManager.On(this.Event, this.Target, callback);

		public void On<T>(Action<ICommunicationMessage, T> callback) => RpcManager.On(this.Event, this.Target, callback);

		public void On<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => RpcManager.On(this.Event, this.Target, callback);

		public void On<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => RpcManager.On(this.Event, this.Target, callback);

		public void On<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => RpcManager.On(this.Event, this.Target, callback);

		public void On<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => RpcManager.On(this.Event, this.Target, callback);
	}
}
