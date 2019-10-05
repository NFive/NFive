using NFive.SDK.Server.Communications;
using NFive.Server.Rpc;
using System;

namespace NFive.Server.Communications
{
	public class CommunicationReceiveClient : ICommunicationReceiveClient
	{
		private readonly IClient target;
		private readonly string @event;

		public CommunicationReceiveClient(string @event)
		{
			this.@event = @event;
		}

		public CommunicationReceiveClient(string @event, IClient client) : this(@event)
		{
			this.target = client;
		}

		public void On(Action<ICommunicationMessage> callback) => RpcManager.On(this.@event, this.target, callback);

		public void On<T>(Action<ICommunicationMessage, T> callback) => RpcManager.On(this.@event, this.target, callback);

		public void On<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => RpcManager.On(this.@event, this.target, callback);

		public void On<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => RpcManager.On(this.@event, this.target, callback);

		public void On<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => RpcManager.On(this.@event, this.target, callback);

		public void On<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => RpcManager.On(this.@event, this.target, callback);

		public void OnRequest(Action<ICommunicationMessage> callback) => RpcManager.OnRequest(this.@event, this.target, callback);

		public void OnRequest<T>(Action<ICommunicationMessage, T> callback) => RpcManager.OnRequest(this.@event, this.target, callback);

		public void OnRequest<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => RpcManager.OnRequest(this.@event, this.target, callback);

		public void OnRequest<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => RpcManager.OnRequest(this.@event, this.target, callback);

		public void OnRequest<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => RpcManager.OnRequest(this.@event, this.target, callback);

		public void OnRequest<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => RpcManager.OnRequest(this.@event, this.target, callback);
	}
}
