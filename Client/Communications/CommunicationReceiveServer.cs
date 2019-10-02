using System;
using NFive.Client.Rpc;
using NFive.SDK.Client.Events;

namespace NFive.Client.Communications
{
	public interface ICommunicationReceive
	{
		void On(Action<ICommunicationMessage> callback);

		void On<T>(Action<ICommunicationMessage, T> callback);

		void On<T1, T2>(Action<ICommunicationMessage, T1, T2> callback);

		void On<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback);

		void On<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback);

		void On<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback);

		void OnRequest(Action<ICommunicationMessage> callback);

		void OnRequest<T>(Action<ICommunicationMessage, T> callback);

		void OnRequest<T1, T2>(Action<ICommunicationMessage, T1, T2> callback);

		void OnRequest<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback);

		void OnRequest<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback);

		void OnRequest<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback);
	}

	public interface ICommunicationReceiveServer : ICommunicationReceive
	{
	}

	public class CommunicationReceiveServer : ICommunicationReceiveServer
	{
		private readonly CommunicationTarget target;

		public CommunicationReceiveServer(CommunicationTarget target)
		{
			this.target = target;
		}

		public void On(Action<ICommunicationMessage> callback) => RpcManager.On(this.target.Event, callback);

		public void On<T>(Action<ICommunicationMessage, T> callback) => RpcManager.On(this.target.Event, callback);

		public void On<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => RpcManager.On(this.target.Event, callback);

		public void On<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => RpcManager.On(this.target.Event, callback);

		public void On<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => RpcManager.On(this.target.Event, callback);

		public void On<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => RpcManager.On(this.target.Event, callback);

		public void OnRequest(Action<ICommunicationMessage> callback) => RpcManager.OnRequest(this.target.Event, callback);

		public void OnRequest<T>(Action<ICommunicationMessage, T> callback) => RpcManager.OnRequest(this.target.Event, callback);

		public void OnRequest<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => RpcManager.OnRequest(this.target.Event, callback);

		public void OnRequest<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => RpcManager.OnRequest(this.target.Event, callback);

		public void OnRequest<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => RpcManager.OnRequest(this.target.Event, callback);

		public void OnRequest<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => RpcManager.OnRequest(this.target.Event, callback);
	}
}
