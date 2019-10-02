using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NFive.Client.Communications;
using NFive.SDK.Client.Events;

namespace NFive.Client.Events
{
	public class EventManager : IEventManager
	{
		private readonly Dictionary<string, List<Subscription>> subscriptions = new Dictionary<string, List<Subscription>>();
		
		public void On(string @event, Action<ICommunicationMessage> action) => InternalOn(@event, action);

		public void On<T>(string @event, Action<ICommunicationMessage, T> action) => InternalOn(@event, action);

		public void On<T1, T2>(string @event, Action<ICommunicationMessage, T1, T2> action) => InternalOn(@event, action);

		public void On<T1, T2, T3>(string @event, Action<ICommunicationMessage, T1, T2, T3> action) => InternalOn(@event, action);

		public void On<T1, T2, T3, T4>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4> action) => InternalOn(@event, action);

		public void On<T1, T2, T3, T4, T5>(string @event, Action<ICommunicationMessage, T1, T2, T3, T4, T5> action) => InternalOn(@event, action);

		public void Emit(string @event, params object[] payload) => InternalRaise(@event, payload);

		public async Task<T1> Request<T1>(string @event, params object[] args) => await InternalRequest<T1>(@event, args);

		public async Task<Tuple<T1, T2>> Request<T1, T2>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2>>(@event, args);

		public async Task<Tuple<T1, T2, T3>> Request<T1, T2, T3>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2, T3>>(@event, args);

		public async Task<Tuple<T1, T2, T3, T4>> Request<T1, T2, T3, T4>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2, T3, T4>>(@event, args);

		public async Task<Tuple<T1, T2, T3, T4, T5>> Request<T1, T2, T3, T4, T5>(string @event, params object[] args) => await InternalRequest<Tuple<T1, T2, T3, T4, T5>>(@event, args);

		private void InternalOn(string @event, Delegate action)
		{
			lock (this.subscriptions)
			{
				if (!this.subscriptions.ContainsKey(@event))
				{
					this.subscriptions.Add(@event, new List<Subscription>());
				}

				this.subscriptions[@event].Add(new Subscription(action));
			}
		}

		private void InternalRaise(string @event, params object[] args)
		{
			lock (this.subscriptions)
			{
				if (!this.subscriptions.ContainsKey(@event)) return;

				foreach (var subscription in this.subscriptions[@event])
				{
					subscription.Handle(args);
				}
			}
		}

		private async Task<TReturn> InternalRequest<TReturn>(string @event, params object[] args)
		{
			var message = new CommunicationMessage(@event);
			var tcs = new TaskCompletionSource<TReturn>();

			try
			{
				InternalOn($"{message.Id}:{@event}", new Action<ICommunicationMessage, TReturn>((e, data) =>
				{
					tcs.SetResult(data);
				}));

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
					this.subscriptions.Remove($"{message.Id}:{@event}");
				}
			}
		}

		public class Subscription
		{
			private readonly Delegate handler;

			public Subscription(Delegate handler)
			{
				this.handler = handler;
			}

			public bool Handle(params object[] args)
			{
				var cancel = false;

				this.handler.DynamicInvoke(args);

				return cancel;
			}
		}
	}
}
