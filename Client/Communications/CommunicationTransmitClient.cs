using JetBrains.Annotations;
using NFive.Client.Rpc;
using NFive.SDK.Client.Events;
using System;
using System.Threading.Tasks;

namespace NFive.Client.Communications
{
	public interface ICommunicationTransmit
	{
		void Emit(params object[] payloads);

		Task<T1> Request<T1>(params object[] payloads);

		Task<Tuple<T1, T2>> Request<T1, T2>(params object[] payloads);

		Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(params object[] payloads);

		Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(params object[] payloads);

		Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(params object[] payloads);
	}

	public interface ICommunicationTransmitClient : ICommunicationTransmit
	{
	}

	[PublicAPI]
	public class CommunicationTransmitClient : ICommunicationTransmitClient
	{
		public string Event { get; }

		public IClient Target { get; }

		public IEventManager EventManager { get; }

		public CommunicationTransmitClient(ICommunicationTarget target)
		{
			this.Event = target.Event;
			this.EventManager = target.EventManager;
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
