using NFive.SDK.Core.Models.Player;
using NFive.SDK.Server.Communications;
using NFive.SDK.Server.Rpc;
using NFive.Server.Rpc;
using NFive.Server.Storage;
using System;
using System.Linq;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Server.Events;
using NFive.Server.Diagnostics;

namespace NFive.Server.Communications
{
	public class CommunicationMessage : ICommunicationMessage
	{
		private readonly IEventManager eventManager;
		private readonly Lazy<User> user;
		private readonly Lazy<Session> session;

		public Guid Id { get; } = Guid.NewGuid();

		public string Event { get; }

		public IClient Client { get; }

		public User User => this.user.Value;

		public Session Session => this.session.Value;

		public CommunicationMessage(string @event)
		{
			this.Event = @event;
		}

		public CommunicationMessage(ICommunicationTarget target)
		{
			this.eventManager = target.EventManager;
			this.Event = target.Event;
		}

		public CommunicationMessage(ICommunicationTarget target, IClient client) : this(target)
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

		public CommunicationMessage(string @event, Guid id, IClient client) : this(@event, client)
		{
			this.Id = id;
		}

		public CommunicationMessage(string @event, IClient client) : this(@event)
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
			if (this.Client == null)
			{
				this.eventManager.Emit(this.Id + ":" + this.Event, payloads);
			}
			else
			{
				new Logger(LogLevel.Trace, "Comms").Warn(this.Id + ":" + this.Event);
				RpcManager.Emit(this.Id + ":" + this.Event, this.Client, payloads);
			}
		}
	}
}
