using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Events;
using NFive.Server.Communications;
using NFive.Server.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NFive.Server.Events
{
	public class EventManager : IEventManager
	{
		private readonly Logger logger;
		private readonly Dictionary<string, List<Delegate>> subscriptions = new Dictionary<string, List<Delegate>>();

		public EventManager(LogLevel level)
		{
			this.logger = new Logger(level, "Events");
		}

		public void On(string @event, Action<ICommunicationMessage> action) => InternalOn(@event, action);

		public void On<T>(string @event, Action<ICommunicationMessage, T> action) => InternalOn(@event, action);

		public void On<T1, T2>(string @event, Action<ICommunicationMessage, T1, T2> action) => InternalOn(@event, action);

		public void On<T1, T2, T3>(string @event, Action<ICommunicationMessage, T1, T2, T3> action) => InternalOn(@event, action);

		public void On<T1, T2, T3, T4>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4> action) => InternalOn(@event, action);

		public void On<T1, T2, T3, T4, T5>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4, T5> action) => InternalOn(@event, action);

		public async Task<T1> Request<T1>(string @event, params object[] args) => await InternalRequest<T1>(@event, args);

		public async Task<Tuple<T1, T2>> Request<T1, T2>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2>>(@event, args);

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2, T3>>(@event, args);

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2, T3, T4>>(@event, args);

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2, T3, T4, T5>>(@event, args);

		public void OnRequest(string @event, Action<ICommunicationMessage> action) { } // TODO

		public void OnRequest<T>(string @event, Action<ICommunicationMessage, T> action) { }

		public void OnRequest<T1, T2>(string @event, Action<ICommunicationMessage, T1, T2> action) { }

		public void OnRequest<T1, T2, T3>(string @event, Action<ICommunicationMessage, T1, T2, T3> action) { }

		public void OnRequest<T1, T2, T3, T4>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4> action) { }

		public void OnRequest<T1, T2, T3, T4, T5>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4, T5> action) { }

		public void Off(string @event, Delegate action)
		{
			lock (this.subscriptions)
			{
				if (this.subscriptions.ContainsKey(@event) && this.subscriptions[@event].Contains(action))
				{
					this.subscriptions[@event].Remove(action);
				}
			}
		}

		public void Emit(string @event, params object[] args)
		{
			lock (this.subscriptions)
			{
				if (!this.subscriptions.ContainsKey(@event)) return;

				var message = new CommunicationMessage(@event, this);

				this.logger.Trace(args.Length > 0 ? $"Emit: \"{@event}\" with {args.Length} payload(s): {string.Join(", ", args.Select(a => a?.ToString() ?? "NULL"))}" : $"Emit: \"{@event}\" without payload");

				foreach (var subscription in this.subscriptions[@event])
				{
					var payload = new List<object> { message };
					payload.AddRange(args);

					subscription.DynamicInvoke(payload.ToArray());
				}
			}
		}

		private void InternalOn(string @event, Delegate action)
		{
			lock (this.subscriptions)
			{
				if (!this.subscriptions.ContainsKey(@event))
				{
					this.subscriptions.Add(@event, new List<Delegate>());
				}

				this.subscriptions[@event].Add(action);

				this.logger.Trace($"On: \"{@event}\" attached to \"{action.Method.DeclaringType?.Name}.{action.Method.Name}({string.Join(", ", action.Method.GetParameters().Select(p => p.ParameterType + " " + p.Name))})\"");
			}
		}

		private async Task<TReturn> InternalRequest<TReturn>(string @event, params object[] args)
		{
			var message = new CommunicationMessage(@event, this);
			var tcs = new TaskCompletionSource<TReturn>();

			try
			{
				InternalOn($"{message.Id}:{@event}", new Action<ICommunicationMessage, TReturn>((e, data) =>
				{
					tcs.SetResult(data);
				}));

				this.logger.Trace(args.Length > 0 ? $"Request Emit: \"{@event}\" with {args.Length} payload(s): {string.Join(", ", args.Select(a => a?.ToString() ?? "NULL"))}" : $"Fire: \"{@event}\" without payload");

				lock (this.subscriptions)
				{
					var payload = new List<object> { message };
					payload.AddRange(args);

					this.subscriptions.Single(s => s.Key == @event).Value.Single().DynamicInvoke(payload.ToArray());
				}

				return await tcs.Task;
			}
			finally
			{
				lock (this.subscriptions)
				{
					this.subscriptions.Remove($"{message.Id}:{@event}");
				}
			}
		}
	}
}
