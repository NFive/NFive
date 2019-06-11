using JetBrains.Annotations;
using NFive.SDK.Core.Rpc;
using NFive.SDK.Server.Rpc;
using NFive.Server.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NFive.Server.Rpc
{
	[PublicAPI]
	public class Rpc : IRpc
	{
		private readonly string @event;
		private readonly Logger logger;
		private readonly ClientHandler handler;
		private readonly RpcTrigger trigger;
		private readonly Serializer serializer;
		private IClient target;

		public Rpc(string @event, Logger logger, ClientHandler handler, RpcTrigger trigger, Serializer serializer)
		{
			this.@event = @event;
			this.logger = logger;
			this.handler = handler;
			this.trigger = trigger;
			this.serializer = serializer;
		}

		public IRpc Target(int handle)
		{
			this.target = new Client(handle);
			return this;
		}

		public IRpc Target(IClient client)
		{
			this.target = client;
			return this;
		}

		public void Trigger(params object[] payloads)
		{
			this.trigger.Fire(new OutboundMessage
			{
				Target = this.target,
				Event = this.@event,
				Payloads = payloads.Select(p => this.serializer.Serialize(p)).ToList()
			});
		}

		public void OnRaw(Delegate callback)
		{
			LogCallback(callback);

			this.handler.Attach(this.@event, callback);
		}

		public void On(Action<IRpcEvent> callback)
		{
			LogCallback(callback);

			Attach(callback, m => new object[0]);
		}

		public void On<T>(Action<IRpcEvent, T> callback)
		{
			LogCallback(callback);

			Attach(callback, m => new object[]
			{
				this.serializer.Deserialize<T>(m.Payloads[0])
			});
		}

		public void On<T1, T2>(Action<IRpcEvent, T1, T2> callback)
		{
			LogCallback(callback);

			Attach(callback, m => new object[]
			{
				this.serializer.Deserialize<T1>(m.Payloads[0]),
				this.serializer.Deserialize<T2>(m.Payloads[1])
			});
		}

		public void On<T1, T2, T3>(Action<IRpcEvent, T1, T2, T3> callback)
		{
			LogCallback(callback);

			Attach(callback, m => new object[]
			{
				this.serializer.Deserialize<T1>(m.Payloads[0]),
				this.serializer.Deserialize<T2>(m.Payloads[1]),
				this.serializer.Deserialize<T3>(m.Payloads[2])
			});
		}

		public void On<T1, T2, T3, T4>(Action<IRpcEvent, T1, T2, T3, T4> callback)
		{
			LogCallback(callback);

			Attach(callback, m => new object[]
			{
				this.serializer.Deserialize<T1>(m.Payloads[0]),
				this.serializer.Deserialize<T2>(m.Payloads[1]),
				this.serializer.Deserialize<T3>(m.Payloads[2]),
				this.serializer.Deserialize<T4>(m.Payloads[3])
			});
		}

		public void On<T1, T2, T3, T4, T5>(Action<IRpcEvent, T1, T2, T3, T4, T5> callback)
		{
			LogCallback(callback);

			Attach(callback, m => new object[]
			{
				this.serializer.Deserialize<T1>(m.Payloads[0]),
				this.serializer.Deserialize<T2>(m.Payloads[1]),
				this.serializer.Deserialize<T3>(m.Payloads[2]),
				this.serializer.Deserialize<T4>(m.Payloads[3]),
				this.serializer.Deserialize<T5>(m.Payloads[4])
			});
		}

		public async Task Request(params object[] payloads)
		{
			await MakeRequest(payloads);
		}

		public async Task<T> Request<T>(params object[] payloads)
		{
			var results = await MakeRequest(payloads);

			return this.serializer.Deserialize<T>(results.Payloads[0]);
		}

		public async Task<Tuple<T1, T2>> Request<T1, T2>(params object[] payloads)
		{
			var results = await MakeRequest(payloads);

			return new Tuple<T1, T2>(
				this.serializer.Deserialize<T1>(results.Payloads[0]),
				this.serializer.Deserialize<T2>(results.Payloads[1])
			);
		}

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(params object[] payloads)
		{
			var results = await MakeRequest(payloads);

			return new Tuple<T1, T2, T3>(
				this.serializer.Deserialize<T1>(results.Payloads[0]),
				this.serializer.Deserialize<T2>(results.Payloads[1]),
				this.serializer.Deserialize<T3>(results.Payloads[2])
			);
		}

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(params object[] payloads)
		{
			var results = await MakeRequest(payloads);

			return new Tuple<T1, T2, T3, T4>(
				this.serializer.Deserialize<T1>(results.Payloads[0]),
				this.serializer.Deserialize<T2>(results.Payloads[1]),
				this.serializer.Deserialize<T3>(results.Payloads[2]),
				this.serializer.Deserialize<T4>(results.Payloads[3])
			);
		}

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(params object[] payloads)
		{
			var results = await MakeRequest(payloads);

			return new Tuple<T1, T2, T3, T4, T5>(
				this.serializer.Deserialize<T1>(results.Payloads[0]),
				this.serializer.Deserialize<T2>(results.Payloads[1]),
				this.serializer.Deserialize<T3>(results.Payloads[2]),
				this.serializer.Deserialize<T4>(results.Payloads[3]),
				this.serializer.Deserialize<T5>(results.Payloads[4])
			);
		}

		private async Task<InboundMessage> MakeRequest(params object[] payloads)
		{
			var tcs = new TaskCompletionSource<InboundMessage>();

			var callback = new Action<byte[]>(data =>
			{
				var message = InboundMessage.From(data);

				LogReceived(message);

				tcs.SetResult(message);
			});

			try
			{
				this.handler.Attach(this.@event, callback);

				Trigger(payloads);

				return await tcs.Task;
			}
			finally
			{
				this.handler.Detach(this.@event, callback);
			}
		}

		private void Attach(Delegate callback, Func<InboundMessage, IEnumerable<object>> func)
		{
			this.handler.Attach(this.@event, new Action<byte[]>(data =>
			{
				var message = InboundMessage.From(data);

				if (!message.Event.StartsWith("nfive:log:")) LogReceived(message);

				var args = new List<object>
				{
					new RpcEvent(message.Event, new Client(message.Source))
				};

				args.AddRange(func(message));

				callback.DynamicInvoke(args.ToArray());
			}));
		}

		private void LogReceived(InboundMessage message)
		{
			if (message.Payloads.Count > 0)
			{
				this.logger.Trace($"Received: \"{message.Event}\" from {message.Source} with {message.Payloads.Count} payload{(message.Payloads.Count > 1 ? "s" : string.Empty)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}");
			}
			else
			{
				this.logger.Trace($"Received: \"{message.Event}\" from {message.Source} with no payloads");
			}
		}

		private void LogCallback(Delegate callback)
		{
			this.logger.Trace($"\"{this.@event}\" attached to \"{callback?.Method?.DeclaringType?.Name}.{callback?.Method?.Name}({string.Join(", ", callback?.Method?.GetParameters()?.Select(p => p.ParameterType + " " + p.Name))})\"");
		}
	}
}
