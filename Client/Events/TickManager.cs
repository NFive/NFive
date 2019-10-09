using System;
using System.Threading.Tasks;
using NFive.SDK.Client.Events;

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

		public void On(Action action)
		{
			this.attachCallback(() => Task.Factory.StartNew(action));
		}

		public void On(Func<Task> action)
		{
			this.attachCallback(action);
		}

		public void Off(Action action)
		{
			this.detachCallback(() => Task.Factory.StartNew(action)); // TODO: Test this detaches correctly
		}

		public void Off(Func<Task> action)
		{
			this.detachCallback(action);
		}
	}
}
