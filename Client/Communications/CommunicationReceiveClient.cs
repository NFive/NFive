using NFive.SDK.Client.Events;
using System;

namespace NFive.Client.Communications
{
	public interface ICommunicationReceiveClient : ICommunicationReceive
	{
	}

	public class CommunicationReceiveClient : ICommunicationReceiveClient
	{
		public string Event { get; }

		public IEventManager EventManager { get; }

		public CommunicationReceiveClient(ICommunicationTarget target)
		{
			this.Event = target.Event;
			this.EventManager = target.EventManager;
		}

		public void On(Action<ICommunicationMessage> callback) => this.EventManager.On(this.Event, callback);

		public void On<T>(Action<ICommunicationMessage, T> callback) => this.EventManager.On(this.Event, callback);

		public void On<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => this.EventManager.On(this.Event, callback);

		public void On<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => this.EventManager.On(this.Event, callback);

		public void On<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => this.EventManager.On(this.Event, callback);

		public void On<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => this.EventManager.On(this.Event, callback);

		public void OnRequest(Action<ICommunicationMessage> callback) => this.EventManager.On(this.Event, callback); // TODO OnRequest

		public void OnRequest<T>(Action<ICommunicationMessage, T> callback) => this.EventManager.On(this.Event, callback);

		public void OnRequest<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => this.EventManager.On(this.Event, callback);

		public void OnRequest<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => this.EventManager.On(this.Event, callback);

		public void OnRequest<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => this.EventManager.On(this.Event, callback);

		public void OnRequest<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => this.EventManager.On(this.Event, callback);
	}
}
