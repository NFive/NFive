using CitizenFX.Core;
using NFive.Client.Communications;
using NFive.Client.Diagnostics;
using NFive.SDK.Client.Events;
using NFive.SDK.Core.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NFive.Client.Rpc
{
	public static class RpcManager
	{
		private static readonly Logger Logger = new Logger("RPC");
		private static readonly Serializer Serializer = new Serializer();
		private static EventHandlerDictionary events;

		internal static void Configure(EventHandlerDictionary eventHandler)
		{
			events = eventHandler;
		}

		public static void OnRaw(string @event, Delegate callback)
		{
			Logger.Trace($"OnRaw: \"{@event}\" attached to \"{callback?.Method?.DeclaringType?.Name}.{callback?.Method?.Name}({string.Join(", ", callback?.Method?.GetParameters()?.Select(p => p.ParameterType + " " + p.Name))})\"");

			events[@event] += callback;
		}

		public static void On(string @event, Action<ICommunicationMessage> callback)
		{
			InternalOn(@event, callback, m => new object[0]);
		}

		public static void On<T>(string @event, Action<ICommunicationMessage, T> callback)
		{
			InternalOn(@event, callback, m => new object[]
			{
				Serializer.Deserialize<T>(m.Payloads[0])
			});
		}

		public static void On<T1, T2>(string @event, Action<ICommunicationMessage, T1, T2> callback)
		{
			InternalOn(@event, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1])
			});
		}

		public static void On<T1, T2, T3>(string @event, Action<ICommunicationMessage, T1, T2, T3> callback)
		{
			InternalOn(@event, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1]),
				Serializer.Deserialize<T3>(m.Payloads[2])
			});
		}

		public static void On<T1, T2, T3, T4>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4> callback)
		{
			InternalOn(@event, callback, m => new object[]
			{
				Serializer.Deserialize<T1>(m.Payloads[0]),
				Serializer.Deserialize<T2>(m.Payloads[1]),
				Serializer.Deserialize<T3>(m.Payloads[2]),
				Serializer.Deserialize<T4>(m.Payloads[3])
			});
		}

		public static void On<T1, T2, T3, T4, T5>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4, T5> callback)
		{
			InternalOn(@event, callback, m => new object[]
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
			Logger.Trace($"Off: \"{@event}\" detached from \"{callback?.Method?.DeclaringType?.Name}.{callback?.Method?.Name}({string.Join(", ", callback?.Method?.GetParameters()?.Select(p => p.ParameterType + " " + p.Name))})\"");

			events[@event] -= callback;
		}

		public static async Task Request(string @event, params object[] payloads)
		{
			await InternalRequest(@event, payloads);
		}

		public static async Task<T> Request<T>(string @event, params object[] payloads)
		{
			var results = await InternalRequest(@event, payloads);

			return Serializer.Deserialize<T>(results.Payloads[0]);
		}

		public static async Task<Tuple<T1, T2>> Request<T1, T2>(string @event, params object[] payloads)
		{
			var results = await InternalRequest(@event, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1])
			);
		}

		public static async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(string @event, params object[] payloads)
		{
			var results = await InternalRequest(@event, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1]),
				Serializer.Deserialize<T3>(results.Payloads[2])
			);
		}

		public static async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(string @event, params object[] payloads)
		{
			var results = await InternalRequest(@event, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1]),
				Serializer.Deserialize<T3>(results.Payloads[2]),
				Serializer.Deserialize<T4>(results.Payloads[3])
			);
		}

		public static async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(string @event, params object[] payloads)
		{
			var results = await InternalRequest(@event, payloads);

			return Tuple.Create(
				Serializer.Deserialize<T1>(results.Payloads[0]),
				Serializer.Deserialize<T2>(results.Payloads[1]),
				Serializer.Deserialize<T3>(results.Payloads[2]),
				Serializer.Deserialize<T4>(results.Payloads[3]),
				Serializer.Deserialize<T5>(results.Payloads[4])
			);
		}

		public static async void Emit(string @event, params object[] payloads)
		{
			var message = new OutboundMessage
			{
				Event = @event,
				Payloads = payloads.Select(p => Serializer.Serialize(p)).ToList()
			};

			if (message.Payloads.Count > 0)
			{
				Logger.Warn($"Emit: \"{message.Event}\" with {message.Payloads.Count} payload{(message.Payloads.Count > 1 ? "s" : string.Empty)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}");
			}
			else
			{
				Logger.Warn($"Emit: \"{message.Event}\" with no payloads");
			}

			// Marshall back to the main thread in order to use a native call.
			await BaseScript.Delay(0);

			Logger.Warn($"TriggerServerEvent: {message.Event}");
			BaseScript.TriggerServerEvent(message.Event, message.Pack());
		}

		private static async Task<InboundMessage> InternalRequest(string @event, params object[] payloads)
		{
			var tcs = new TaskCompletionSource<InboundMessage>();

			var callback = new Action<byte[]>(data =>
			{
				Logger.Warn($"Request callback");

				var message = InboundMessage.From(data);

				if (message.Payloads.Count > 0)
				{
					Logger.Warn($"Request Received: \"{message.Event}\" with {message.Payloads.Count} payload{(message.Payloads.Count > 1 ? "s" : string.Empty)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}");
				}
				else
				{
					Logger.Warn($"Request Received: \"{message.Event}\" with no payloads");
				}

				Logger.Warn($"Request callback SetResult");

				tcs.SetResult(message);
			});

			try
			{
				events[@event] += callback;

				Emit(@event, payloads);

				Logger.Warn($"await callback");

				return await tcs.Task;
			}
			finally
			{
				events[@event] -= callback;
			}
		}

		private static void InternalOn(string @event, Delegate callback, Func<InboundMessage, IEnumerable<object>> func)
		{
			Logger.Warn($"On: \"{@event}\" attached to \"{callback.Method?.DeclaringType?.Name}.{callback?.Method?.Name}({string.Join(", ", callback?.Method?.GetParameters()?.Select(p => p.ParameterType + " " + p.Name))})\"");

			events[@event] += new Action<byte[]>(data =>
			{
				var message = InboundMessage.From(data);

				if (message.Payloads.Count > 0)
				{
					Logger.Warn($"On Received: \"{message.Event}\" with {message.Payloads.Count} payload{(message.Payloads.Count > 1 ? "s" : string.Empty)}:{Environment.NewLine}\t{string.Join($"{Environment.NewLine}\t", message.Payloads)}");
				}
				else
				{
					Logger.Warn($"On Received: \"{message.Event}\" with no payloads");
				}

				var args = new List<object>
				{
					new CommunicationMessage(@event)
				};

				args.AddRange(func(message));

				callback.DynamicInvoke(args.ToArray());
			});
		}
	}
}
