using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NFive.SDK.Client.Events;

namespace NFive.Client.Events
{
	public class EventManager : IEventManager
	{
		private readonly Dictionary<string, List<Subscription>> subscriptions = new Dictionary<string, List<Subscription>>();

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

		public void On(string @event, Action action) => InternalOn(@event, action);

		public void On<T>(string @event, Action<T> action) => InternalOn(@event, action);

		public void On<T1, T2>(string @event, Action<T1, T2> action) => InternalOn(@event, action);

		public void On<T1, T2, T3>(string @event, Action<T1, T2, T3> action) => InternalOn(@event, action);

		public void On<T1, T2, T3, T4>(string @event, Action<T1, T2, T3, T4> action) => InternalOn(@event, action);

		public void On<T1, T2, T3, T4, T5>(string @event, Action<T1, T2, T3, T4, T5> action) => InternalOn(@event, action);


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

		public void Raise(string @event) => InternalRaise(@event);

		public void Raise<T>(string @event, T p1) => InternalRaise(@event, p1);

		public void Raise<T1, T2>(string @event, T1 p1, T2 p2) => InternalRaise(@event, p1, p2);

		public void Raise<T1, T2, T3>(string @event, T1 p1, T2 p2, T3 p3) => InternalRaise(@event, p1, p2, p3);

		public void Raise<T1, T2, T3, T4>(string @event, T1 p1, T2 p2, T3 p3, T4 p4) => InternalRaise(@event, p1, p2, p3, p4);

		public void Raise<T1, T2, T3, T4, T5>(string @event, T1 p1, T2 p2, T3 p3, T4 p4, T5 p5) => InternalRaise(@event, p1, p2, p3, p4, p5);


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
		
		public class Subscription
		{
			private readonly Delegate handler;

			public Subscription(Delegate handler)
			{
				this.handler = handler;
			}

			public bool Handle(params object[] args)
			{
				bool cancel = false;

				this.handler.DynamicInvoke(args);

				return cancel;
			}
		}
	}
}
