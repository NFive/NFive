using CitizenFX.Core;
using CitizenFX.Core.Native;
using NFive.SDK.Core.Diagnostics;
using NFive.SDK.Core.Helpers;
using NFive.SDK.Core.Models.Player;
using NFive.SDK.Core.Rpc;
using NFive.SDK.Server;
using NFive.SDK.Server.Controllers;
using NFive.SDK.Server.Events;
using NFive.SDK.Server.Rcon;
using NFive.SDK.Server.Rpc;
using NFive.Server.Configuration;
using NFive.Server.Rpc;
using NFive.Server.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NFive.Server.Controllers
{
	public class SessionController : ConfigurableController<SessionConfiguration>
	{
		private readonly List<Action> sessionCallbacks = new List<Action>();
		private readonly ConcurrentDictionary<Guid, Session> sessions = new ConcurrentDictionary<Guid, Session>();
		private readonly object threadLock = new object();
		private readonly ConcurrentDictionary<Session, Tuple<Task, CancellationTokenSource>> threads = new ConcurrentDictionary<Session, Tuple<Task, CancellationTokenSource>>();

		private Player CurrentHost { get; set; }

		public SessionController(ILogger logger, IEventManager events, IRpcHandler rpc, IRconManager rcon, SessionConfiguration configuration) : base(logger, events, rpc, rcon, configuration)
		{
			API.EnableEnhancedHostSupport(true);

			this.Events.On(ServerEvents.ServerInitialized, OnSeverInitialized);

			this.Rpc.Event("hostingSession").OnRaw(new Action<Player>(OnHostingSession));
			this.Rpc.Event("HostedSession").OnRaw(new Action<Player>(OnHostedSession));
			this.Rpc.Event("playerConnecting").OnRaw(new Action<Player, string, CallbackDelegate, ExpandoObject>(Connecting));
			this.Rpc.Event("playerDropped").OnRaw(new Action<Player, string, CallbackDelegate>(Dropped));

			this.Rpc.Event(RpcEvents.ClientInitialize).On<string>(Initialize);
			this.Rpc.Event(RpcEvents.ClientInitialized).On(Initialized);
			this.Rpc.Event(SessionEvents.DisconnectPlayer).On<string>(Disconnect);

			this.Events.OnRequest(SessionEvents.GetMaxPlayers, () => this.Configuration.MaxClients);
			this.Events.OnRequest(SessionEvents.GetCurrentSessionsCount, () => this.sessions.Count);
			this.Events.OnRequest(SessionEvents.GetCurrentSessions, () => this.sessions.ToList());
		}

		private void OnSeverInitialized()
		{
			var lastActive = this.Events.Request<DateTime>(BootEvents.GetLastActiveTime);
			using (var context = new StorageContext())
			using (var transaction = context.Database.BeginTransaction())
			{
				lastActive = lastActive == default(DateTime) ? DateTime.UtcNow : lastActive;
				var disconnectedSessions = context.Sessions.Where(s => s.Disconnected == null).ToList();
				foreach (var disconnectedSession in disconnectedSessions)
				{
					disconnectedSession.Disconnected = lastActive;
					disconnectedSession.DisconnectReason = "Session killed, disconnect time set to last server active time";
					context.Sessions.AddOrUpdate(disconnectedSession);
				}

				context.SaveChanges();
				transaction.Commit();
			}
		}

		private async void OnHostingSession([FromSource] Player player)
		{
			if (this.CurrentHost != null)
			{
				player.TriggerEvent("sessionHostResult", "wait");

				this.sessionCallbacks.Add(() => player.TriggerEvent("sessionHostResult", "free"));

				return;
			}

			string hostId;

			try
			{
				hostId = API.GetHostId();
			}
			catch (NullReferenceException)
			{
				hostId = null;
			}

			if (!string.IsNullOrEmpty(hostId) && API.GetPlayerLastMsg(API.GetHostId()) < 1000)
			{
				player.TriggerEvent("sessionHostResult", "conflict");

				return;
			}

			this.sessionCallbacks.Clear();
			this.CurrentHost = player;

			this.Logger.Debug($"Game host is now {this.CurrentHost.Handle} \"{this.CurrentHost.Name}\"");

			player.TriggerEvent("sessionHostResult", "go");

			await BaseScript.Delay(5000);

			this.sessionCallbacks.ForEach(c => c());
			this.CurrentHost = null;
		}

		private void OnHostedSession([FromSource] Player player)
		{
			if (this.CurrentHost != null && this.CurrentHost != player) return;

			this.sessionCallbacks.ForEach(c => c());
			this.CurrentHost = null;
		}

		private async void Connecting([FromSource] Player player, string playerName, CallbackDelegate drop, ExpandoObject callbacks)
		{
			var client = new Client(int.Parse(player.Handle));
			var deferrals = new Deferrals(callbacks, drop);
			Session session = null;
			User user = null;

			await this.Events.RaiseAsync(SessionEvents.ClientConnecting, client, deferrals);

			using (var context = new StorageContext())
			using (var transaction = context.Database.BeginTransaction())
			{
				context.Configuration.ProxyCreationEnabled = false;
				context.Configuration.LazyLoadingEnabled = false;

				try
				{
					user = context.Users.SingleOrDefault(u => u.License == client.License);

					if (user == default(User))
					{
						await this.Events.RaiseAsync(SessionEvents.UserCreating, client);
						// Create user
						user = new User
						{
							Id = GuidGenerator.GenerateTimeBasedGuid(),
							License = client.License,
							SteamId = client.SteamId,
							Name = client.Name
						};

						context.Users.Add(user);
						await context.SaveChangesAsync();
						await this.Events.RaiseAsync(SessionEvents.UserCreated, client, user);
					}
					else
					{
						// Update details
						user.Name = client.Name;
						if (client.SteamId.HasValue) user.SteamId = client.SteamId;
					}

					await this.Events.RaiseAsync(SessionEvents.SessionCreating, client);
					// Create session
					session = new Session
					{
						Id = GuidGenerator.GenerateTimeBasedGuid(),
						User = user,
						IpAddress = client.EndPoint,
						Created = DateTime.UtcNow
					};

					context.Sessions.Add(session);

					// Save changes
					await context.SaveChangesAsync();
					transaction.Commit();
				}
				catch (DbEntityValidationException ex)
				{
					var errorMessages = ex.EntityValidationErrors
						.SelectMany(x => x.ValidationErrors)
						.Select(x => x.ErrorMessage);

					var fullErrorMessage = string.Join("; ", errorMessages);

					var exceptionMessage = string.Concat(ex.Message, " The Validation errors are: ", fullErrorMessage);
					transaction.Rollback();
					throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
				}
				catch (Exception ex)
				{
					transaction.Rollback();

					this.Logger.Error(ex);
				}
			}

			if (user == null || session == null) throw new Exception($"Failed to create session for {player.Name}");

			this.sessions[session.Id] = session;
			var threadCancellationToken = new CancellationTokenSource();
			lock (this.threads)
			{
				this.threads.TryAdd(
					session,
					new Tuple<Task, CancellationTokenSource>(Task.Factory.StartNew(() => MonitorSession(session, client), threadCancellationToken.Token), threadCancellationToken)
				);
			}

			await this.Events.RaiseAsync(SessionEvents.SessionCreated, client, session, deferrals);

			if (this.sessions.Any(s => s.Value.User.Id == user.Id && s.Key != session.Id)) Reconnecting(client, session);

			await this.Events.RaiseAsync(SessionEvents.ClientConnected, client, session);
			this.Logger.Debug($"[{session.Id}] Player \"{user.Name}\" connected from {session.IpAddress}");
		}

		private async void Reconnecting(Client client, Session session)
		{
			this.Logger.Trace($"Client reconnecting: {session.UserId}");
			var oldSession = this.sessions.Select(s => s.Value).OrderBy(s => s.Created).FirstOrDefault(s => s.User.Id == session.UserId);
			if (oldSession == null) return;
			await this.Events.RaiseAsync(SessionEvents.ClientReconnecting, client, session, oldSession);

			lock (this.threadLock)
			{
				var oldThread = this.threads.OrderBy(t => t.Key.Created).FirstOrDefault(t => t.Key.UserId == session.UserId).Key;
				if (oldThread != null)
				{

					this.Logger.Trace($"Disposing of old thread: {oldThread.User.Name}");
					this.threads[oldThread].Item2.Cancel();
					this.threads[oldThread].Item1.Wait();
					this.threads[oldThread].Item2.Dispose();
					this.threads[oldThread].Item1.Dispose();
					this.threads.TryRemove(oldThread, out _);
				}
			}

			this.sessions.TryRemove(oldSession.Id, out oldSession);
			await this.Events.RaiseAsync(SessionEvents.ClientReconnected, client, session, oldSession);
		}

		private void Dropped([FromSource] Player player, string disconnectMessage, CallbackDelegate drop)
		{
			this.Logger.Debug($"Player Dropped: {player.Name} | Reason: {disconnectMessage}");

			var client = new Client(int.Parse(player.Handle));

			Disconnecting(client, disconnectMessage);
		}

		private void Disconnect(IRpcEvent e, string reason)
		{
			API.DropPlayer(e.Client.Handle.ToString(), reason);
		}

		private async void Disconnecting(Client client, string disconnectMessage)
		{
			await this.Events.RaiseAsync(SessionEvents.ClientDisconnecting, client);

			using (var context = new StorageContext())
			{
				context.Configuration.LazyLoadingEnabled = false;

				var session = this.sessions.Select(s => s.Value).OrderBy(s => s.Created).FirstOrDefault(s => s.User.License == client.License && s.Disconnected == null && s.DisconnectReason == null);
				if (session == null) throw new Exception($"No session to end for disconnected user \"{client.Name}\""); // TODO: SessionException

				session.Disconnected = DateTime.UtcNow;
				session.DisconnectReason = disconnectMessage;
				context.Sessions.AddOrUpdate(session);
				await context.SaveChangesAsync();

				lock (this.threadLock)
				{
					var oldThread = this.threads.SingleOrDefault(t => t.Key.UserId == session.UserId).Key;
					if (oldThread != null)
					{
						this.threads[oldThread].Item2.Cancel();
						this.threads[oldThread].Item1.Wait();
						this.threads[oldThread].Item2.Dispose();
						this.threads[oldThread].Item1.Dispose();
						this.threads.TryRemove(oldThread, out _);
					}
					var threadCancellationToken = new CancellationTokenSource();
					this.threads.TryAdd(
						session,
						Tuple.Create<Task, CancellationTokenSource>(Task.Factory.StartNew(() => MonitorSession(session, client), threadCancellationToken.Token), threadCancellationToken)
					);
				}
				await this.Events.RaiseAsync(SessionEvents.ClientDisconnected, client, session);

				this.Logger.Debug($"[{session.Id}] Player \"{client.Name}\" disconnected: {session.DisconnectReason}");
			}
		}

		private async void Initialize(IRpcEvent e, string clientVersion)
		{
			var client = new Client(e.Client.Handle);

			await this.Events.RaiseAsync(SessionEvents.ClientInitializing, client);

			e.Reply(e.User);
		}

		private async void Initialized(IRpcEvent e)
		{
			this.Logger.Trace($"Client Initialized: {e.Client.Name}");

			var client = new Client(e.Client.Handle);
			var session = this.sessions.Select(s => s.Value).Single(s => s.User.Id == e.User.Id);

			using (var context = new StorageContext())
			using (var transaction = context.Database.BeginTransaction())
			{
				session.Connected = DateTime.UtcNow;

				context.Sessions.AddOrUpdate(session);
				await context.SaveChangesAsync();
				transaction.Commit();
			}

			this.Events.Raise(SessionEvents.ClientInitialized, client, session);
		}

		private async Task MonitorSession(Session session, Client client)
		{
			while (session.IsConnected && this.threads.ContainsKey(session) && !this.threads[session].Item2.Token.IsCancellationRequested)
			{
				await BaseScript.Delay(100);

				if (API.GetPlayerLastMsg(client.Handle.ToString()) <= this.Configuration.ConnectionTimeout.TotalMilliseconds) continue;

				await this.Events.RaiseAsync(SessionEvents.SessionTimedOut, client, session);
				session.Disconnected = DateTime.UtcNow;
				Disconnecting(client, "Session Timed Out");
			}

			this.Logger.Trace("Starting reconnect grace checks");

			while (DateTime.UtcNow.Subtract(session.Disconnected ?? DateTime.UtcNow) < this.Configuration.ReconnectGrace && this.threads.ContainsKey(session) && !this.threads[session].Item2.Token.IsCancellationRequested)
			{
				await BaseScript.Delay(100);
			}

			this.sessions.TryRemove(session.Id, out session);
		}
	}
}
