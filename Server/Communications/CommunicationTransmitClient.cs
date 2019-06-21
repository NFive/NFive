using System;
using System.Threading.Tasks;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Rpc;
using NFive.Server.Rpc;

namespace NFive.Server.Communications
{
	public class CommunicationTransmitClient : ICommunicationTransmitClient
	{
		// TODO: Specific client targeting.
		private readonly IClient targetClient;
		private readonly CommunicationTarget target;
		private readonly IRpc rpc;

		public CommunicationTransmitClient(CommunicationTarget target)
		{
			this.target = target;
			this.rpc = RpcManager.Event(target.Event);
		}

		public CommunicationTransmitClient(CommunicationTarget target, IClient client) : this(target)
		{
			this.targetClient = client;
		}

		public void Emit(params object[] payloads)
		{
			this.rpc.Trigger(payloads);
		}

		public async Task<T> Request<T>(params object[] payloads)
		{
			return await this.rpc.Request<T>(payloads);
		}

		public async Task<Tuple<T1, T2>> Request<T1, T2>(params object[] payloads)
		{
			return await this.rpc.Request<T1, T2>(payloads);
		}

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(params object[] payloads)
		{
			return await this.rpc.Request<T1, T2, T3>(payloads);
		}

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(params object[] payloads)
		{
			return await this.rpc.Request<T1, T2, T3, T4>(payloads);
		}

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(params object[] payloads)
		{
			return await this.rpc.Request<T1, T2, T3, T4, T5>(payloads);
		}
	}
}
