using NFive.SDK.Client.Events;
using System;
using System.Threading.Tasks;

namespace NFive.Client.Events
{
	public class TickManager : ITickManager
	{
		private readonly Action<Func<Task>> attachCallback;
		private readonly Action<Func<Task>> detachCallback;

		public TickManager(Action<Func<Task>> attachCallback, Action<Func<Task>> detachCallback)
		{
			this.attachCallback = attachCallback;
			this.detachCallback = detachCallback;
		}

		public void On(Action callback)
		{
			this.attachCallback(() => Task.Factory.StartNew(callback));
		}

		public void On(Func<Task> callback)
		{
			this.attachCallback(callback);
		}

		public void Off(Action callback)
		{
			this.detachCallback(() => Task.Factory.StartNew(callback));
		}

		public void Off(Func<Task> callback)
		{
			this.detachCallback(callback);
		}
	}
}
