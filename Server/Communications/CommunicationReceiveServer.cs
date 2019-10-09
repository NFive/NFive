using System;
using NFive.SDK.Server.Communications;
using NFive.Server.Events;

namespace NFive.Server.Communications
{
	public class CommunicationReceiveServer : ICommunicationReceiveServer
	{
		private readonly string @event;
		private readonly EventManager eventManager;

		public CommunicationReceiveServer(string @event, EventManager eventManager)
		{
			this.@event = @event;
			this.eventManager = eventManager;
		}

		public void On(Action<ICommunicationMessage> callback) => this.eventManager.On(this.@event, callback);

		public void On<T>(Action<ICommunicationMessage, T> callback) => this.eventManager.On(this.@event, callback);

		public void On<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => this.eventManager.On(this.@event, callback);

		public void On<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => this.eventManager.On(this.@event, callback);

		public void On<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => this.eventManager.On(this.@event, callback);

		public void On<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => this.eventManager.On(this.@event, callback);

		public void OnRequest(Action<ICommunicationMessage> callback) => this.eventManager.OnRequest(this.@event, callback);

		public void OnRequest<T>(Action<ICommunicationMessage, T> callback) => this.eventManager.OnRequest(this.@event, callback);

		public void OnRequest<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => this.eventManager.OnRequest(this.@event, callback);

		public void OnRequest<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => this.eventManager.OnRequest(this.@event, callback);

		public void OnRequest<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => this.eventManager.OnRequest(this.@event, callback);

		public void OnRequest<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => this.eventManager.OnRequest(this.@event, callback);
	}
}
