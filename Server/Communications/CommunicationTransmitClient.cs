using JetBrains.Annotations;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Rpc;
using NFive.Server.Rpc;
using System;
using System.Threading.Tasks;

namespace NFive.Server.Communications
{
	[PublicAPI]
	public class CommunicationTransmitClient : ICommunicationTransmitClient
	{
		public string Event { get; }

		public IClient Target { get; }

		public CommunicationTransmitClient(ICommunicationTarget target)
		{
			this.Event = target.Event;
		}

		public CommunicationTransmitClient(ICommunicationTarget target, IClient client) : this(target)
		{
			this.Target = client;
		}

		public void Emit(params object[] payloads) => RpcManager.Emit(this.Event, this.Target, payloads);

		public async Task<T> Request<T>(params object[] payloads) => await RpcManager.Request<T>(this.Event, this.Target, payloads);

		public async Task<Tuple<T1, T2>> Request<T1, T2>(params object[] payloads) => await RpcManager.Request<T1, T2>(this.Event, this.Target, payloads);

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(params object[] payloads) => await RpcManager.Request<T1, T2, T3>(this.Event, this.Target, payloads);

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(params object[] payloads) => await RpcManager.Request<T1, T2, T3, T4>(this.Event, this.Target, payloads);

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(params object[] payloads) => await RpcManager.Request<T1, T2, T3, T4, T5>(this.Event, this.Target, payloads);
	}
}
