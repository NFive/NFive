using System;
using System.Threading.Tasks;
using NFive.SDK.Server.Communications;
using NFive.Server.Events;

namespace NFive.Server.Communications
{
	public class CommunicationTransmitServer : ICommunicationTransmitServer
	{
		private readonly string @event;
		private readonly EventManager eventManager;

		public CommunicationTransmitServer(string @event, EventManager eventManager)
		{
			this.@event = @event;
			this.eventManager = eventManager;
		}

		public void Emit(params object[] payloads)
		{
			this.eventManager.Emit(this.@event, payloads);
		}

		public async Task<T> Request<T>(params object[] payloads) => await this.eventManager.Request<T>(this.@event, payloads);

		public async Task<Tuple<T1, T2>> Request<T1, T2>(params object[] payloads) => await this.eventManager.Request<T1, T2>(this.@event, payloads);

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(params object[] payloads) => await this.eventManager.Request<T1, T2, T3>(this.@event, payloads);

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(params object[] payloads) => await this.eventManager.Request<T1, T2, T3, T4>(this.@event, payloads);

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(params object[] payloads) => await this.eventManager.Request<T1, T2, T3, T4, T5>(this.@event, payloads);
	}
}
