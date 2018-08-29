using System;
using System.Threading.Tasks;
using NFive.SDK.Client.Events;

namespace NFive.Client.Events
{
	public class TickManager : ITickManager
	{
		private readonly Action<Func<Task>> attachCallback;
		private readonly Action<Func<Task>> deachCallback;

		public TickManager(Action<Func<Task>> attachCallback, Action<Func<Task>> deachCallback)
		{
			this.attachCallback = attachCallback;
			this.deachCallback = deachCallback;
		}

		public void Attach(Func<Task> callback)
		{
			this.attachCallback(callback);
		}

		public void Detach(Func<Task> callback)
		{
			this.deachCallback(callback);
		}
	}
}
