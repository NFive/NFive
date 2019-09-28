using System;
using System.Linq;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Rpc;
using NFive.Server.Rpc;
using NFive.Server.Storage;

namespace NFive.Server.Communications
{
	public class CommunicationMessage : ICommunicationMessage
	{
		private readonly Lazy<User> user;

		private readonly Lazy<Session> session;

		private readonly CommunicationTarget target;

		public Guid Id { get; }

		public IClient Client { get; }
		
		public User User => this.user.Value;

		public Session Session => this.session.Value;

		public CommunicationMessage(CommunicationTarget target)
		{
			this.target = target;
			this.Id = Guid.NewGuid();
		}

		public CommunicationMessage(CommunicationTarget target, IClient client) : this(target)
		{
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
			if (this.Client == null) ReplyToServer(payloads);
			else ReplyToClient(payloads);
		}

		private void ReplyToClient(params object[] payloads)
		{
			RpcManager.Event(this.Id + ":" + this.target.Event).Target(this.Client).Trigger(payloads);
		}

		private void ReplyToServer(params object[] payloads)
		{
			this.target.EventManager.Fire(this.Id + ":" + this.target.Event, payloads);
		}
	}
}
