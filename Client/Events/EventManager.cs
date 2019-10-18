using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NFive.Client.Communications;
using NFive.Client.Diagnostics;
using NFive.SDK.Client.Communications;

namespace NFive.Client.Events
{
	[PublicAPI]
	public class EventManager
	{
		private readonly Logger logger;
		private readonly Dictionary<string, List<Delegate>> subscriptions = new Dictionary<string, List<Delegate>>();

		public EventManager()
		{
			this.logger = new Logger("Events");
		}

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

				this.logger.Trace(args.Length > 0
					? $"Emit: \"{@event}\" with {args.Length} payload(s): {string.Join(", ", args.Select(a => a?.ToString() ?? "NULL"))}"
					: $"Emit: \"{@event}\" without payload");

				foreach (var subscription in this.subscriptions[@event])
				{
					var payload = new List<object> { message };
					payload.AddRange(args);

					subscription.DynamicInvoke(payload.ToArray());
				}
			}
		}

		internal void On(string @event, Delegate action)
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

		internal void OnRequest(string @event, Delegate action)
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

		internal async Task<TReturn> Request<TReturn>(string @event, params object[] args)
		{
			var message = new CommunicationMessage(@event, this);
			var tcs = new TaskCompletionSource<TReturn>();

			try
			{
				On($"{message.Id}:{@event}", new Action<ICommunicationMessage, TReturn>((e, data) =>
				{
					this.logger.Trace($"Request Reply: \"{@event}\" with {args.Length} payload(s): {string.Join(", ", args.Select(a => a?.ToString() ?? "NULL"))}");

					tcs.SetResult(data);
				}));

				this.logger.Trace(args.Length > 0
					? $"Request Emit: \"{@event}\" with {args.Length} payload(s): {string.Join(", ", args.Select(a => a?.ToString() ?? "NULL"))}"
					: $"Request Emit: \"{@event}\" without payload");

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
