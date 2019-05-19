using System;
using System.Collections.Generic;
using System.Linq;
using NFive.SDK.Core.Rpc;
using NFive.SDK.Server.Rpc;
using NFive.Server.Diagnostics;

namespace NFive.Server.Rpc
{
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
			throw new NotImplementedException();
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

		[Obsolete]
		public void Trigger(IClient client, params object[] payloads)
		{
			this.trigger.Fire(new OutboundMessage
			{
				Target = client,
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

		private void Attach(Delegate callback, Func<InboundMessage, IEnumerable<object>> func)
		{
			this.handler.Attach(this.@event, new Action<string>(json =>
			{
				var message = this.serializer.Deserialize<InboundMessage>(json);
				message.Received = DateTime.UtcNow;

				this.logger.Trace($"Received {message.Event} from {message.Source} with {message.Payloads.Count} payloads(s)");

				var args = new List<object>
				{
					new RpcEvent(message.Event, new Client(message.Source))
				};

				args.AddRange(func(message));

				callback.DynamicInvoke(args.ToArray());
			}));
		}

		private void LogCallback(Delegate callback)
		{
			this.logger.Trace($"\"{this.@event}\" attached to \"{callback.Method.DeclaringType?.Name}.{callback.Method.Name}({string.Join(", ", callback.Method.GetParameters().Select(p => p.ParameterType + " " + p.Name))})\"");
		}
	}
}
