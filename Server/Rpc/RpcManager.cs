using CitizenFX.Core;
using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Rpc;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Rpc;
using NFive.Server.Communications;
using NFive.Server.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NFive.Server.Rpc
{
	public static class RpcManager
	{
		private static Logger logger;
		private static readonly Serializer Serializer = new Serializer();
		private static EventHandlerDictionary events;
		private static PlayerList players;

		internal static void Configure(LogLevel level, EventHandlerDictionary eventHandler, PlayerList playerList)
		{
			logger = new Logger(level, "RPC");
			events = eventHandler;
			players = playerList;
		}

		public static void OnRaw(string @event, Delegate callback)
		{
			logger.Trace($"OnRaw: \"{@event}\" attached to \"{callback?.Method?.DeclaringType?.Name}.{callback?.Method?.Name}({string.Join(", ", callback?.Method?.GetParameters()?.Select(p => p.ParameterType + " " + p.Name))})\"");

			events[@event] += callback;
		}

		public static void On(string @event, [CanBeNull]IClient target, Action<ICommunicationMessage> callback)
		{
			InternalOn(@event, target, callback, m => new object[0]);
		}

		public static void On<T>(string @event, [CanBeNull]IClient target, Action<ICommunicationMessage, T> callback)
		{
			InternalOn(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T>(m.Payloads[0])
			});
		}

		public static void On<T1, T2>(string @event, [CanBeNull]IClient target, Action<ICommunicationMessage, T1, T2> callback)
		{
			InternalOn(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1])
			});
		}

		public static void On<T1, T2, T3>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T1, T2, T3> callback)
		{
			InternalOn(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1]),
				Serializer.Deserialize<T3>(m.Payloads[2])
			});
		}

		public static void On<T1, T2, T3, T4>(string @event, [CanBeNull]IClient target, Action<ICommunicationMessage, T1, T2, T3, T4> callback)
		{
			InternalOn(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1]),
				Serializer.Deserialize<T3>(m.Payloads[2]),
				Serializer.Deserialize<T4>(m.Payloads[3])
			});
		}

		public static void On<T1, T2, T3, T4, T5>(string @event, [CanBeNull]IClient target, Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback)
		{
			InternalOn(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1]),
				Serializer.Deserialize<T3>(m.Payloads[2]),
				Serializer.Deserialize<T4>(m.Payloads[3]),
				Serializer.Deserialize<T5>(m.Payloads[4])
			});
		}

		public static void Off(string @event, Delegate callback)
		{
			logger.Trace($"Off: \"{@event}\" detached from \"{callback?.Method?.DeclaringType?.Name}.{callback?.Method?.Name}({string.Join(", ", callback?.Method?.GetParameters()?.Select(p => p.ParameterType + " " + p.Name))})\"");

			events[@event] -= callback;
		}

		public static async Task Request(string @event, [CanBeNull]IClient target, params object[] payloads)
		{
			await InternalRequest(@event, target, payloads);
		}

		public static async Task<T> Request<T>(string @event, [CanBeNull]IClient target, params object[] payloads)
		{
			var results = await InternalRequest(@event, target, payloads);

			return Serializer.Deserialize<T>(results.Payloads[0]);
		}

		public static async Task<Tuple<T1, T2>> Request<T1, T2>(string @event, [CanBeNull]IClient target, params object[] payloads)
		{
			var results = await InternalRequest(@event, target, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1])
			);
		}

		public static async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(string @event, [CanBeNull]IClient target, params object[] payloads)
		{
			var results = await InternalRequest(@event, target, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1]),
				Serializer.Deserialize<T3>(results.Payloads[2])
			);
		}

		public static async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(string @event, [CanBeNull]IClient target, params object[] payloads)
		{
			var results = await InternalRequest(@event, target, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1]),
				Serializer.Deserialize<T3>(results.Payloads[2]),
				Serializer.Deserialize<T4>(results.Payloads[3])
			);
		}

		public static async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(string @event, [CanBeNull]IClient target, params object[] payloads)
		{
			var results = await InternalRequest(@event, target, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1]),
				Serializer.Deserialize<T3>(results.Payloads[2]),
				Serializer.Deserialize<T4>(results.Payloads[3]),
				Serializer.Deserialize<T5>(results.Payloads[4])
			);
		}

		public static async void Emit(string @event, [CanBeNull] IClient target, OutboundMessage message)
		{
			if (message.Payloads.Count > 0)
			{
				logger.Trace($"Fire: \"{message.Event}\" {(message.Target != null ? $"to {message.Target.Handle} " : string.Empty)}with {message.Payloads.Count} payload{(message.Payloads.Count > 1 ? "s" : string.Empty)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}");
			}
			else
			{
				logger.Trace($"Fire: \"{message.Event}\" {(message.Target != null ? $"to {message.Target.Handle} " : string.Empty)}with no payloads");
			}

			// Marshall back to the main thread in order to use a native call.
			await BaseScript.Delay(0);

			if (message.Target != null)
			{
				BaseScript.TriggerClientEvent(new PlayerList()[message.Target.Handle], message.Event, message.Pack());
			}
			else
			{
				BaseScript.TriggerClientEvent(message.Event, message.Pack());
			}
		}
		
		public static async void Emit(string @event, [CanBeNull] IClient target, params object[] payloads)
		{
			Emit(@event, target, new OutboundMessage
			{
				Id = Guid.NewGuid(),
				Target = target,
				Event = @event,
				Payloads = payloads.Select(p => Serializer.Serialize(p)).ToList()
			});
		}

		private static async Task<InboundMessage> InternalRequest(string @event, [CanBeNull]IClient target, params object[] payloads)
		{
			var tcs = new TaskCompletionSource<InboundMessage>();

			var callback = new Action<byte[]>(data =>
			{
				var message = InboundMessage.From(data);

				if (message.Payloads.Count > 0)
				{
					logger.Trace($"Request Received: \"{message.Event}\" with {message.Payloads.Count} payload{(message.Payloads.Count > 1 ? "s" : string.Empty)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}");
				}
				else
				{
					logger.Trace($"Request Received: \"{message.Event}\" with no payloads");
				}

				tcs.SetResult(message);
			});

			var msg = new OutboundMessage
			{
				Id = Guid.NewGuid(),
				Target = target,
				Event = @event,
				Payloads = payloads.Select(p => Serializer.Serialize(p)).ToList()
			};

			try
			{
				events[$"{msg.Id}:{@event}"] += callback;

				Emit(@event, target, msg);

				return await tcs.Task;
			}
			finally
			{
				events[$"{msg.Id}:{@event}"] -= callback;
			}
		}

		private static void InternalOn(string @event, IClient target, Delegate callback, Func<InboundMessage, IEnumerable<object>> func)
		{
			logger.Trace($"On: \"{@event}\" attached to \"{callback.Method?.DeclaringType?.Name}.{callback?.Method?.Name}({string.Join(", ", callback?.Method?.GetParameters()?.Select(p => p.ParameterType + " " + p.Name))})\"");

			events[@event] += new Action<byte[]>(data =>
			{
				var message = InboundMessage.From(data);

				if (!message.Event.StartsWith("nfive:log:"))
				{
					if (message.Payloads.Count > 0)
					{
						logger.Trace($"On Received: \"{message.Event}\" with {message.Payloads.Count} payload{(message.Payloads.Count > 1 ? "s" : string.Empty)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}");
					}
					else
					{
						logger.Trace($"On Received: \"{message.Event}\" with no payloads");
					}
				}

				var args = new List<object>
				{
					new CommunicationMessage(@event, new Client(message.Source))
				};

				args.AddRange(func(message));

				callback.DynamicInvoke(args.ToArray());
			});
		}
	}
}
