using NFive.SDK.Server.Communications;
using System;

namespace NFive.Server.Communications
{
	public class CommunicationReceiveServer : ICommunicationReceiveServer
	{
		private readonly CommunicationTarget target;

		public CommunicationReceiveServer(CommunicationTarget target)
		{
			this.target = target;
		}

		public void On(Action<ICommunicationMessage> callback) => this.target.EventManager.On(this.target.Event, callback);

		public void On<T>(Action<ICommunicationMessage, T> callback) => this.target.EventManager.On(this.target.Event, callback);

		public void On<T1, T2>(Action<ICommunicationMessage, T1, T2> callback) => this.target.EventManager.On(this.target.Event, callback);

		public void On<T1, T2, T3>(Action<ICommunicationMessage, T1, T2, T3> callback) => this.target.EventManager.On(this.target.Event, callback);

		public void On<T1, T2, T3, T4>(Action<ICommunicationMessage, T1, T2, T3, T4> callback) => this.target.EventManager.On(this.target.Event, callback);

		public void On<T1, T2, T3, T4, T5>(Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback) => this.target.EventManager.On(this.target.Event, callback);
	}
}
