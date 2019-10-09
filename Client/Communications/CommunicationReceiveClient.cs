using System;
using JetBrains.Annotations;
using NFive.Client.Events;
using NFive.SDK.Client.Communications;

namespace NFive.Client.Communications
{
	[PublicAPI]
	public class CommunicationReceiveClient : ICommunicationReceiveClient
	{
		public string Event { get; }

		public EventManager EventManager { get; }

		public CommunicationReceiveClient(string @event, EventManager eventManager)
		{
			this.Event = @event;
			this.EventManager = eventManager;
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
