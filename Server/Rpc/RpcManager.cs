using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using JetBrains.Annotations;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Rpc;
using NFive.SDK.Server.Communications;
using NFive.Server.Communications;
using NFive.Server.Diagnostics;

namespace NFive.Server.Rpc
{
	[PublicAPI]
	public static class RpcManager
	{
		private static readonly Serializer Serializer = new Serializer();
		private static Logger logger;
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
			logger.Trace($"OnRaw: \"{@event}\" attached to \"{PrintCallback(callback)}\"");

			events[@event] += callback;
		}

		public static void On(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage> callback)
		{
			InternalOn(@event, target, callback, m => new object[0]);
		}

		public static void On<T>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T> callback)
		{
			InternalOn(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T>(m.Payloads[0])
			});
		}

		public static void On<T1, T2>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T1, T2> callback)
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

		public static void On<T1, T2, T3, T4>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T1, T2, T3, T4> callback)
		{
			InternalOn(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1]),
				Serializer.Deserialize<T3>(m.Payloads[2]),
				Serializer.Deserialize<T4>(m.Payloads[3])
			});
		}

		public static void On<T1, T2, T3, T4, T5>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback)
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

		public static void OnRequest(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage> callback)
		{
			InternalOnRequest(@event, target, callback, m => new object[0]);
		}

		public static void OnRequest<T>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T> callback)
		{
			InternalOnRequest(@event, target, callback, m =>
			{
				logger.Trace($"Got payload: {m.Payloads[0]}, converting to type {typeof(T).FullName}");
				logger.Trace($"Got message: {new Serializer().Serialize(m)}");

				return new object[]
				{
					Serializer.Deserialize<T>(m.Payloads[0])
				};
			});
		}

		public static void OnRequest<T1, T2>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T1, T2> callback)
		{
			InternalOnRequest(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1])
			});
		}

		public static void OnRequest<T1, T2, T3>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T1, T2, T3> callback)
		{
			InternalOnRequest(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1]),
				Serializer.Deserialize<T3>(m.Payloads[2])
			});
		}

		public static void OnRequest<T1, T2, T3, T4>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T1, T2, T3, T4> callback)
		{
			InternalOnRequest(@event, target, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1]),
				Serializer.Deserialize<T3>(m.Payloads[2]),
				Serializer.Deserialize<T4>(m.Payloads[3])
			});
		}

		public static void OnRequest<T1, T2, T3, T4, T5>(string @event, [CanBeNull] IClient target, Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback)
		{
			InternalOnRequest(@event, target, callback, m => new object[]
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
			logger.Trace($"Off: \"{@event}\" detached from \"{PrintCallback(callback)}\"");

			events[@event] -= callback;
		}

		public static async Task Request(string @event, [CanBeNull] IClient target, params object[] payloads)
		{
			await InternalRequest(@event, target, payloads);
		}

		public static async Task<T> Request<T>(string @event, [CanBeNull] IClient target, params object[] payloads)
		{
			var results = await InternalRequest(@event, target, payloads);

			return Serializer.Deserialize<T>(results.Payloads[0]);
		}

		public static async Task<Tuple<T1, T2>> Request<T1, T2>(string @event, [CanBeNull] IClient target, params object[] payloads)
		{
			var results = await InternalRequest(@event, target, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1])
			);
		}

		public static async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(string @event, [CanBeNull] IClient target, params object[] payloads)
		{
			var results = await InternalRequest(@event, target, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1]),
				Serializer.Deserialize<T3>(results.Payloads[2])
			);
		}

		public static async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(string @event, [CanBeNull] IClient target, params object[] payloads)
		{
			var results = await InternalRequest(@event, target, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1]),
				Serializer.Deserialize<T3>(results.Payloads[2]),
				Serializer.Deserialize<T4>(results.Payloads[3])
			);
		}

		public static async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(string @event, [CanBeNull] IClient target, params object[] payloads)
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

		private static async void Emit(OutboundMessage message)
		{
			logger.Trace($"Fire: \"{PrintOutboundMessage(message)}\"");

			// Marshall back to the main thread in order to use a native call.
			await BaseScript.Delay(0);

			if (message.Target != null)
			{
				logger.Trace($"TriggerClientEvent: Using PlayerList with {"player".Pluralize(players.Count())}");
				BaseScript.TriggerClientEvent(players[message.Target.Handle], message.Event, message.Pack());
			}
			else
			{
				logger.Trace("TriggerClientEvent: All clients");
				BaseScript.TriggerClientEvent(message.Event, message.Pack());
			}
		}

		public static void Emit(string @event, [CanBeNull] IClient target, params object[] payloads)
		{
			Emit(new OutboundMessage
			{
				Id = Guid.NewGuid(),
				Target = target,
				Event = @event,
				Payloads = payloads.Select(p => Serializer.Serialize(p)).ToList()
			});
		}

		private static async Task<InboundMessage> InternalRequest(string @event, [CanBeNull] IClient target, params object[] payloads)
		{
			var tcs = new TaskCompletionSource<InboundMessage>();

			var callback = new Action<byte[]>(data =>
			{
				var message = InboundMessage.From(data);

				logger.Trace($"Request Received: {PrintInboundMessage(message)}");

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

				Emit(msg);

				return await tcs.Task;
			}
			finally
			{
				events[$"{msg.Id}:{@event}"] -= callback;
			}
		}

		private static void InternalOn(string @event, IClient target, Delegate callback, Func<InboundMessage, IEnumerable<object>> func)
		{
			logger.Trace($"On: \"{@event}\" attached to \"{PrintCallback(callback)}\"");

			events[@event] += new Action<byte[]>(data =>
			{
				var message = InboundMessage.From(data);

				if (target != null && message.Source != target.Handle)
				{
					logger.Trace($"Ignoring event {@event} triggered by: {message.Source} | expected: {target.Handle}");
					return;
				}

				if (!message.Event.StartsWith("nfive:log:")) logger.Trace($"OnRequest Received: {PrintInboundMessage(message)}");

				var args = new List<object>
				{
					new CommunicationMessage(@event, new Client(message.Source))
				};

				args.AddRange(func(message));

				callback.DynamicInvoke(args.ToArray());
			});
		}

		private static void InternalOnRequest(string @event, IClient target, Delegate callback, Func<InboundMessage, IEnumerable<object>> func)
		{
			logger.Trace($"OnRequest: \"{@event}\" attached to \"{PrintCallback(callback)}\"");

			events[@event] += new Action<byte[]>(data =>
			{
				var message = InboundMessage.From(data);

				if (target != null && message.Source != target.Handle)
				{
					logger.Trace($"Ignoring event {@event} triggered by: {message.Source} | expected: {target.Handle}");
					return;
				}

				if (!message.Event.StartsWith("nfive:log:")) logger.Trace($"OnRequest Received: {PrintInboundMessage(message)}");

				var args = new List<object>
				{
					new CommunicationMessage(@event, message.Id, new Client(message.Source))
				};

				args.AddRange(func(message));

				logger.Trace($"DynamicInvoke: {PrintCallback(callback)} with {string.Join(", ", args.ToString())}");

				callback.DynamicInvoke(args.ToArray());
			});
		}

		// TODO: Move from client SDK to core
		public static string Pluralize(this string str, int value, string extension = "s", CultureInfo culture = null)
		{
			var val = value.ToString(culture ?? CultureInfo.InvariantCulture);
			return value == 1 ? $"{val} {str}" : $"{val} {str}{extension}";
		}

		private static string PrintCallback(Delegate callback)
		{
			return $"{callback.Method.DeclaringType?.Name}.{callback.Method.Name}({string.Join(", ", callback.Method.GetParameters().Select(p => p.ParameterType + " " + p.Name))})";
		}

		private static string PrintInboundMessage(InboundMessage message)
		{
			var str = $"\"{message.Event}\" with ";

			if (message.Payloads.Count < 1) return str + "no payloads";

			return str + $"{"payload".Pluralize(message.Payloads.Count)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}";
		}

		private static string PrintOutboundMessage(OutboundMessage message)
		{
			var str = $"\"{message.Event}\" with ";

			if (message.Target != null) str += $"to {message.Target.Handle} ";

			if (message.Payloads.Count < 1) return str + "no payloads";

			return str + $"{"payload".Pluralize(message.Payloads.Count)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}";
		}
	}
}
