using System;
using System.Linq;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Server.Rpc;
using NFive.Server.Storage;

namespace NFive.Server.Rpc
{
	public class RpcEvent : IRpcEvent
	{
		private readonly Lazy<User> user;

		public string Event { get; }

		public IClient Client { get; }

		public User User => this.user.Value;

		public RpcEvent(string @event, IClient client)
		{
			this.Event = @event;
			this.Client = client;

			this.user = new Lazy<User>(() =>
			{
				using (var context = new StorageContext())
				{
					context.Configuration.ProxyCreationEnabled = false;
					context.Configuration.LazyLoadingEnabled = false;

					return context.Users.Single(u => u.License == this.Client.License);
				}
			});
		}

		public void Reply(params object[] payloads)
		{
			this.Client.Event(this.Event).Trigger(this.Client, payloads);
		}
	}
}
