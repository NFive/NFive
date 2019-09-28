using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Server.Events;
using NFive.Server.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NFive.SDK.Server.Communications;
using NFive.Server.Communications;

namespace NFive.Server.Events
{
	public class EventManager : IEventManager
	{
		private readonly Logger logger;
		private readonly Dictionary<string, List<Subscription>> subscriptions = new Dictionary<string, List<Subscription>>();

		public EventManager(LogLevel level)
		{
			this.logger = new Logger(level, "Events");
		}

		private void InternalOn(string @event, Delegate action)
		{
			lock (this.subscriptions)
			{
				if (!this.subscriptions.ContainsKey(@event))
				{
					this.subscriptions.Add(@event, new List<Subscription>());
				}

				this.subscriptions[@event].Add(new Subscription(action));
				LogAttach(@event, action);
			}
		}

		private void InternalOff(string @event, Subscription action)
		{
			lock (this.subscriptions)
			{
				if (this.subscriptions.ContainsKey(@event) && this.subscriptions[@event].Contains(action))
				{
					this.subscriptions[@event].Remove(action);
				}
			}
		}

		public void On(string @event, Action<ICommunicationMessage> action) => InternalOn(@event, action);

		public void On<T>(string @event, Action<ICommunicationMessage, T> action) => InternalOn(@event, action);

		public void On<T1, T2>(string @event, Action<ICommunicationMessage, T1, T2> action) => InternalOn(@event, action);

		public void On<T1, T2, T3>(string @event, Action<ICommunicationMessage, T1, T2, T3> action) => InternalOn(@event, action);

		public void On<T1, T2, T3, T4>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4> action) => InternalOn(@event, action);

		public void On<T1, T2, T3, T4, T5>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4, T5> action) => InternalOn(@event, action);

		public void OnRequest<TReturn>(string @event, Func<TReturn> action) => InternalOn("request:" + @event, action);

		public void OnRequest<T1, TReturn>(string @event, Func<T1, TReturn> action) => InternalOn("request:" + @event, action);

		public void OnRequest<T1, T2, TReturn>(string @event, Func<T1, T2, TReturn> action) => InternalOn("request:" + @event, action);

		public void OnRequest<T1, T2, T3, TReturn>(string @event, Func<T1, T2, T3, TReturn> action) => InternalOn("request:" + @event, action);

		public void OnRequest<T1, T2, T3, T4, TReturn>(string @event, Func<T1, T2, T3, T4, TReturn> action) => InternalOn("request:" + @event, action);

		public void OnRequest<T1, T2, T3, T4, T5, TReturn>(string @event, Func<T1, T2, T3, T4, T5, TReturn> action) => InternalOn("request:" + @event, action);


		private void InternalRaise(string @event, params object[] args)
		{
			LogCall(@event, args);
			lock (this.subscriptions)
			{
				if (!this.subscriptions.ContainsKey(@event)) return;

				var message = new CommunicationMessage(new CommunicationTarget(this, @event));

				foreach (var subscription in this.subscriptions[@event])
				{
					subscription.Handle(message, args);
				}
			}
		}

		public void Raise(string @event) => InternalRaise(@event);

		public void Raise<T>(string @event, T p1) => InternalRaise(@event, p1);

        public void Raise(string @event, params object[] args) => InternalRaise(@event, args);

        public void Raise<T1, T2>(string @event, T1 p1, T2 p2) => InternalRaise(@event, p1, p2);

		public void Raise<T1, T2, T3>(string @event, T1 p1, T2 p2, T3 p3) => InternalRaise(@event, p1, p2, p3);

		public void Raise<T1, T2, T3, T4>(string @event, T1 p1, T2 p2, T3 p3, T4 p4) => InternalRaise(@event, p1, p2, p3, p4);

		public void Raise<T1, T2, T3, T4, T5>(string @event, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => InternalRaise(@event, p1, p2, p3, p4, p5);

		public void Fire(string @event, params object[] payload) => InternalRaise(@event, payload);


		private async Task InternalRaiseAsync(string @event, params object[] args)
		{
			await Task.Factory.StartNew(() =>
			{
				InternalRaise(@event, args);
			});
		}

		public Task RaiseAsync(string @event) => InternalRaiseAsync(@event);

		public Task RaiseAsync<T>(string @event, T p1) => InternalRaiseAsync(@event, p1);

		public Task RaiseAsync<T1, T2>(string @event, T1 p1, T2 p2) => InternalRaiseAsync(@event, p1, p2);

		public Task RaiseAsync<T1, T2, T3>(string @event, T1 p1, T2 p2, T3 p3) => InternalRaiseAsync(@event, p1, p2, p3);

		public Task RaiseAsync<T1, T2, T3, T4>(string @event, T1 p1, T2 p2, T3 p3, T4 p4) => InternalRaiseAsync(@event, p1, p2, p3, p4);

		public Task RaiseAsync<T1, T2, T3, T4, T5>(string @event, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => InternalRaiseAsync(@event, p1, p2, p3, p4, p5);


		private async Task<TReturn> InternalRequest<TReturn>(string @event, params object[] args)
		{
			LogCall(@event, args);
			var message = new CommunicationMessage(new CommunicationTarget(this, @event));
			var tcs = new TaskCompletionSource<TReturn>();

			var callback = new Action<ICommunicationMessage, TReturn>((e, data) =>
			{
				tcs.SetResult(data);
			});

			try
			{
				InternalOn(message.Id + ":" + @event, callback);

				lock (this.subscriptions)
				{
					this.subscriptions.Single(s => s.Key == @event).Value.Single().Handle(message, args);
				}

				return await tcs.Task;
			}
			finally
			{
				lock (this.subscriptions)
				{
					this.subscriptions.Remove(@event);
				}
			}
		}

		public async Task<T1> Request<T1>(string @event, params object[] args) => await InternalRequest<T1>(@event, args);

		public async Task<Tuple<T1, T2>> Request<T1, T2>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2>>(@event, args);

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2, T3>>(@event, args);

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2, T3, T4>>(@event, args);

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2, T3, T4, T5>>(@event, args);


		private void LogAttach(string @event, Delegate callback)
		{
			this.logger.Trace($"\"{@event}\" attached to \"{callback.Method.DeclaringType?.Name}.{callback.Method.Name}({string.Join(", ", callback.Method.GetParameters().Select(p => p.ParameterType + " " + p.Name))})\"");
		}

		private void LogCall(string @event, params object[] args)
		{
			this.logger.Trace(args.Length > 0
				? $"Fire: \"{@event}\" with {args.Length} payload(s): {string.Join(", ", args.Select(a => a?.ToString() ?? "NULL"))}"
				: $"Fire: \"{@event}\" without payload");
		}

		private class Subscription
		{
			private readonly Delegate handler;

			public Subscription(Delegate handler)
			{
				this.handler = handler;
			}

			public bool Handle(CommunicationMessage message, params object[] args)
			{
				// TODO: Implement this

				var cancel = false;

				var payload = new List<object> { message };
				payload.AddRange(args);
				this.handler.DynamicInvoke(payload.ToArray());

				return cancel;
			}
		}
	}
}
