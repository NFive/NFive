using JetBrains.Annotations;
using NFive.Client.Rpc;
using NFive.SDK.Client.Events;
using System;
using System.Threading.Tasks;
using NFive.SDK.Client.Communications;

namespace NFive.Client.Communications
{

	[PublicAPI]
	public class CommunicationTransmitClient : ICommunicationTransmitClient
	{
		public string Event { get; }

		public IEventManager EventManager { get; }

		public CommunicationTransmitClient(ICommunicationTarget target)
		{
			this.Event = target.Event;
			this.EventManager = target.EventManager;
		}

		public void Emit(params object[] payloads) => this.EventManager.Emit(this.Event, payloads);

		public async Task<T> Request<T>(params object[] payloads) => await this.EventManager.Request<T>(this.Event, payloads);

		public async Task<Tuple<T1, T2>> Request<T1, T2>(params object[] payloads) => await this.EventManager.Request<T1, T2>(this.Event, payloads);

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(params object[] payloads) => await this.EventManager.Request<T1, T2, T3>(this.Event, payloads);

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(params object[] payloads) => await this.EventManager.Request<T1, T2, T3, T4>(this.Event, payloads);

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(params object[] payloads) => await this.EventManager.Request<T1, T2, T3, T4, T5>(this.Event, payloads);
	}
}
