using JetBrains.Annotations;
using NFive.SDK.Server.Communications;
using NFive.Server.Rpc;
using System;
using System.Threading.Tasks;

namespace NFive.Server.Communications
{
	[PublicAPI]
	public class CommunicationTransmitClient : ICommunicationTransmitClient
	{
		private readonly string @event;
		private readonly IClient target;

		public CommunicationTransmitClient(string @event)
		{
			this.@event = @event;
		}

		public CommunicationTransmitClient(string @event, IClient client) : this(@event)
		{
			this.target = client;
		}

		public void Emit(params object[] payloads) => RpcManager.Emit(this.@event, this.target, payloads);

		public async Task<T> Request<T>(params object[] payloads) => await RpcManager.Request<T>(this.@event, this.target, payloads);

		public async Task<Tuple<T1, T2>> Request<T1, T2>(params object[] payloads) => await RpcManager.Request<T1, T2>(this.@event, this.target, payloads);

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(params object[] payloads) => await RpcManager.Request<T1, T2, T3>(this.@event, this.target, payloads);

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(params object[] payloads) => await RpcManager.Request<T1, T2, T3, T4>(this.@event, this.target, payloads);

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(params object[] payloads) => await RpcManager.Request<T1, T2, T3, T4, T5>(this.@event, this.target, payloads);
	}
}
