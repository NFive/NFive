using System;
using System.Linq;
using JetBrains.Annotations;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Server.Communications;
using NFive.Server.Events;
using NFive.Server.Rpc;
using NFive.Server.Storage;

namespace NFive.Server.Communications
{
	[PublicAPI]
	public class CommunicationMessage : ICommunicationMessage
	{
		private readonly EventManager eventManager;
		private readonly Lazy<Session> session;
		private readonly Lazy<User> user;

		public Guid Id { get; } = Guid.NewGuid();

		public string Event { get; }

		public IClient Client { get; }

		public User User => this.user.Value;

		public Session Session => this.session.Value;

		public CommunicationMessage(string @event)
		{
			this.Event = @event;
		}

		public CommunicationMessage(string @event, EventManager eventManager) : this(@event)
		{
			this.eventManager = eventManager;
		}

		public CommunicationMessage(string @event, IClient client) : this(@event)
		{
			this.Client = client;

			this.user = new Lazy<User>(() =>
			{
				using (var context = new StorageContext())
				{
					context.ChangeTracker.LazyLoadingEnabled = false;

					return context.Users.Single(u => u.License == this.Client.License);
				}
			});

			this.session = new Lazy<Session>(() =>
			{
				using (var context = new StorageContext())
				{
					context.ChangeTracker.LazyLoadingEnabled = false;

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

		public void Reply(params object[] payloads)
		{
			if (this.Client == null)
			{
				this.eventManager.Emit($"{this.Id}:{this.Event}", payloads);
			}
			else
			{
				RpcManager.Emit($"{this.Id}:{this.Event}", this.Client, payloads);
			}
		}
	}
}
