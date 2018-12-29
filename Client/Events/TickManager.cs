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

		public void Attach(Action callback)
		{
			this.attachCallback(() => Task.Factory.StartNew(callback));
		}

		public void Attach(Func<Task> callback)
		{
			this.attachCallback(callback);
		}

		public void Detach(Action callback)
		{
			this.detachCallback(() => Task.Factory.StartNew(callback));
		}

		public void Detach(Func<Task> callback)
		{
			this.detachCallback(callback);
		}
	}
}
