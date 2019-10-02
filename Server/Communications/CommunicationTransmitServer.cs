using NFive.SDK.Server.Communications;
using System;
using System.Threading.Tasks;

namespace NFive.Server.Communications
{
	public class CommunicationTransmitServer : ICommunicationTransmitServer
	{
		private readonly CommunicationTarget target;

		public CommunicationTransmitServer(CommunicationTarget target)
		{
			this.target = target;
		}

		public void Emit(params object[] payloads) => this.target.EventManager.Emit(this.target.Event, payloads);

		public async Task<T> Request<T>(params object[] payloads) => await this.target.EventManager.Request<T>(this.target.Event, payloads);

		public async Task<Tuple<T1, T2>> Request<T1, T2>(params object[] payloads) => await this.target.EventManager.Request<T1, T2>(this.target.Event, payloads);

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(params object[] payloads) => await this.target.EventManager.Request<T1, T2, T3>(this.target.Event, payloads);

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(params object[] payloads) => await this.target.EventManager.Request<T1, T2, T3, T4>(this.target.Event, payloads);

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(params object[] payloads) => await this.target.EventManager.Request<T1, T2, T3, T4, T5>(this.target.Event, payloads);
	}
}
