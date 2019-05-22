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
		private readonly Lazy<Session> session;

		public string Event { get; }

		public IClient Client { get; }

		public User User => this.user.Value;

		public Session Session => this.session.Value;

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

			this.session = new Lazy<Session>(() =>
			{
				using (var context = new StorageContext())
				{
					context.Configuration.ProxyCreationEnabled = false;
					context.Configuration.LazyLoadingEnabled = false;

					var clientSession = context.Sessions.Single(s => s.UserId == this.User.Id && s.Disconnected == null);
					clientSession.Handle = client.Handle;

					return clientSession;
				}
			});
		}

		public void Reply(params object[] payloads)
		{
			RpcManager.Event(this.Event).Target(this.Client).Trigger(payloads);
		}
	}
}
