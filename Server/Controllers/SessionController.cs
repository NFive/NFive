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
using NFive.SDK.Server.Communications;

namespace NFive.Server.Controllers
{
	public class SessionController : ConfigurableController<SessionConfiguration>
	{
		private readonly ICommunicationManager Comms;
		private readonly List<Action> sessionCallbacks = new List<Action>();
		private readonly ConcurrentDictionary<Guid, Session> sessions = new ConcurrentDictionary<Guid, Session>();
		private readonly object threadLock = new object();
		private readonly ConcurrentDictionary<Session, Tuple<Task, CancellationTokenSource>> threads = new ConcurrentDictionary<Session, Tuple<Task, CancellationTokenSource>>();

		private int? CurrentHost { get; set; }

		public SessionController(ILogger logger, IEventManager events, IRpcHandler rpc, IRconManager rcon, SessionConfiguration configuration, ICommunicationManager comms) : base(logger, events, rpc, rcon, configuration)
		{
			this.Comms = comms;
		}

		public override Task Loaded()
		{
			// Rebroadcast raw FiveM RPC events as wrapped server events
			this.Rpc.Event("hostingSession").OnRaw(new Action<Player>(OnHostingSessionRaw));
			this.Rpc.Event("HostedSession").OnRaw(new Action<Player>(OnHostedSessionRaw));
			this.Rpc.Event("playerConnecting").OnRaw(new Action<Player, string, CallbackDelegate, ExpandoObject>(OnPlayerConnectingRaw));
			this.Rpc.Event("playerDropped").OnRaw(new Action<Player, string, CallbackDelegate>(OnPlayerDroppedRaw));


			this.Comms.Event("nfive:server:hostedSession").FromServer().On<IClient>(OnHostedSession);
			//this.Events.On<IClient>("nfive:server:hostedSession", OnHostedSession);
			
			this.Comms.Event("nfive:server:playerConnecting").FromServer().On<IClient, string, CallbackDelegate, ExpandoObject>(OnConnecting);
			//this.Events.On<IClient, string, CallbackDelegate, ExpandoObject>("nfive:server:playerConnecting", OnConnecting);

			this.Comms.Event("nfive:server:playerDropped").FromServer().On<IClient, string, CallbackDelegate>(OnDropped);
			//this.Events.On<IClient, string, CallbackDelegate>("nfive:server:playerDropped", OnDropped);


			this.Comms.Event(RpcEvents.ClientInitialize).FromClients().On<string>(Initialize);
			//this.Rpc.Event(RpcEvents.ClientInitialize).On<string>(Initialize);

			this.Comms.Event(RpcEvents.ClientInitialized).FromClients().On(Initialized);
			//this.Rpc.Event(RpcEvents.ClientInitialized).On(Initialized);

			this.Comms.Event(SessionEvents.DisconnectPlayer).FromClients().On<string>(Disconnect);
			//this.Rpc.Event(SessionEvents.DisconnectPlayer).On<string>(Disconnect);

			
			this.Comms.Event(ServerEvents.ServerInitialized).FromServer().On(OnSeverInitialized);
			//this.Events.On(ServerEvents.ServerInitialized, OnSeverInitialized);


			this.Comms.Event(SessionEvents.GetMaxPlayers).FromServer().On(e => e.Reply(this.Configuration.MaxClients));
			//this.Events.OnRequest(SessionEvents.GetMaxPlayers, () => this.Configuration.MaxClients);

			this.Comms.Event(SessionEvents.GetCurrentSessionsCount).FromServer().On(e => e.Reply(this.sessions.Count));
			//this.Events.OnRequest(SessionEvents.GetCurrentSessionsCount, () => this.sessions.Count);

			this.Comms.Event(SessionEvents.GetCurrentSessions).FromServer().On(e => e.Reply(this.sessions.ToList()));
			//this.Events.OnRequest(SessionEvents.GetCurrentSessions, () => this.sessions.ToList());


			return base.Loaded();
		}

		public override Task Started()
		{
			API.EnableEnhancedHostSupport(true);

			return base.Started();
		}

		private async void OnHostingSessionRaw([FromSource] Player player)
		{
			var client = new Client(player.Handle);

			// TODO: Add to SessionEvents
			this.Comms.Event("nfive:server:hostingSession").ToServer().Emit(client);
			//this.Events.Raise("nfive:server:hostingSession", client);

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
			this.CurrentHost = client.Handle;

			this.Logger.Debug($"Game host is now {this.CurrentHost}");

			player.TriggerEvent("sessionHostResult", "go");

			await BaseScript.Delay(5000);

			this.sessionCallbacks.ForEach(c => c());
			this.CurrentHost = null;
		}

		private void OnHostedSessionRaw([FromSource] Player player)
		{
			// TODO: Add to SessionEvents
			this.Comms.Event("nfive:server:hostedSession").ToServer().Emit(new Client(player.Handle));
			//this.Events.Raise("nfive:server:hostedSession", new Client(player.Handle));
		}

		private void OnPlayerConnectingRaw([FromSource] Player player, string playerName, CallbackDelegate drop, ExpandoObject callbacks)
		{
			// TODO: Add to SessionEvents
			this.Comms.Event("nfive:server:playerConnecting").ToServer().Emit(new Client(player.Handle), playerName, drop, callbacks);
			//this.Events.Raise("nfive:server:playerConnecting", new Client(player.Handle), playerName, drop, callbacks);
		}

		private void OnPlayerDroppedRaw([FromSource] Player player, string disconnectMessage, CallbackDelegate drop)
		{
			// TODO: Add to SessionEvents
			this.Comms.Event("nfive:server:playerDropped").ToServer().Emit(new Client(player.Handle), disconnectMessage, drop);
			//this.Events.Raise("nfive:server:playerDropped", new Client(player.Handle), disconnectMessage, drop);
		}

		private async void OnSeverInitialized(ICommunicationMessage e)
		{
			var lastActive = await this.Comms.Event(BootEvents.GetLastActiveTime).ToServer().Request<DateTime>();

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

		private void OnHostedSession(ICommunicationMessage e, IClient client)
		{
			if (this.CurrentHost != null && this.CurrentHost != client.Handle) return;

			this.sessionCallbacks.ForEach(c => c());
			this.CurrentHost = null;
		}

		private async void OnConnecting(ICommunicationMessage e, IClient client, string playerName, CallbackDelegate drop, ExpandoObject callbacks)
		{
			var deferrals = new Deferrals(callbacks, drop);
			Session session = null;
			User user = null;

			this.Comms.Event(SessionEvents.ClientConnecting).ToServer().Emit(client, deferrals);
			//await this.Events.RaiseAsync(SessionEvents.ClientConnecting, client, deferrals);

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
						this.Comms.Event(SessionEvents.UserCreating).ToServer().Emit(client);
						//await this.Events.RaiseAsync(SessionEvents.UserCreating, client);

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

						this.Comms.Event(SessionEvents.UserCreated).ToServer().Emit(client, user);
						//await this.Events.RaiseAsync(SessionEvents.UserCreated, client, user);
					}
					else
					{
						// Update details
						user.Name = client.Name;
						if (client.SteamId.HasValue) user.SteamId = client.SteamId;
					}

					this.Comms.Event(SessionEvents.SessionCreating).ToServer().Emit(client);
					//await this.Events.RaiseAsync(SessionEvents.SessionCreating, client);

					// Create session
					session = new Session
					{
						Id = GuidGenerator.GenerateTimeBasedGuid(),
						User = user,
						IpAddress = client.EndPoint,
						Created = DateTime.UtcNow,
						Handle = client.Handle,
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

			if (user == null || session == null) throw new Exception($"Failed to create session for {client.Name}");

			this.sessions[session.Id] = session;
			var threadCancellationToken = new CancellationTokenSource();
			lock (this.threads)
			{
				this.threads.TryAdd(
					session,
					new Tuple<Task, CancellationTokenSource>(Task.Factory.StartNew(() => MonitorSession(session, client), threadCancellationToken.Token), threadCancellationToken)
				);
			}

			this.Comms.Event(SessionEvents.SessionCreated).ToServer().Emit(client, session, deferrals);
			//await this.Events.RaiseAsync(SessionEvents.SessionCreated, client, session, deferrals);

			if (this.sessions.Any(s => s.Value.User.Id == user.Id && s.Key != session.Id)) Reconnecting(client, session);

			this.Comms.Event(SessionEvents.ClientConnected).ToServer().Emit(client, session);
			//await this.Events.RaiseAsync(SessionEvents.ClientConnected, client, session);

			this.Logger.Debug($"[{session.Id}] Player \"{user.Name}\" connected from {session.IpAddress}");
		}

		private void Reconnecting(IClient client, Session session)
		{
			this.Logger.Trace($"Client reconnecting: {session.UserId}");
			var oldSession = this.sessions.Select(s => s.Value).OrderBy(s => s.Created).FirstOrDefault(s => s.User.Id == session.UserId);
			if (oldSession == null) return;

			this.Comms.Event(SessionEvents.ClientReconnecting).ToServer().Emit(client, session, oldSession);
			//await this.Events.RaiseAsync(SessionEvents.ClientReconnecting, client, session, oldSession);

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

			this.Comms.Event(SessionEvents.ClientReconnected).ToServer().Emit(client, session, oldSession);
			//await this.Events.RaiseAsync(SessionEvents.ClientReconnected, client, session, oldSession);
		}

		private void OnDropped(ICommunicationMessage e, IClient client, string disconnectMessage, CallbackDelegate drop)
		{
			this.Logger.Debug($"Player Dropped: {client.Name} | Reason: {disconnectMessage}");

			Disconnecting(client, disconnectMessage);
		}

		private void Disconnect(ICommunicationMessage e, string reason)
		{
			API.DropPlayer(e.Client.Handle.ToString(), reason);
		}

		private async void Disconnecting(IClient client, string disconnectMessage)
		{
			this.Comms.Event(SessionEvents.ClientDisconnecting).ToServer().Emit(client);
			//await this.Events.RaiseAsync(SessionEvents.ClientDisconnecting, client);

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

				this.Comms.Event(SessionEvents.ClientDisconnected).ToServer().Emit(client, session);
				//await this.Events.RaiseAsync(SessionEvents.ClientDisconnected, client, session);

				this.Logger.Debug($"[{session.Id}] Player \"{client.Name}\" disconnected: {session.DisconnectReason}");
			}
		}

		private void Initialize(ICommunicationMessage e, string clientVersion)
		{
			if (clientVersion != typeof(Program).Assembly.GetName().Version.ToString())
			{
				this.Logger.Warn($"Client version does not match server version, got {clientVersion}, expecting {typeof(Program).Assembly.GetName().Version}, dropping client: {e.Client.Handle}");

				API.DropPlayer(e.Client.Handle.ToString(), $"Please reconnect to get the latest NFive client version");

				return;
			}

			this.Comms.Event(SessionEvents.ClientInitializing).ToServer().Emit(e.Client);
			//await this.Events.RaiseAsync(SessionEvents.ClientInitializing, e.Client);

			e.Reply(e.User, ServerLogConfiguration.Output.ClientConsole, ServerLogConfiguration.Output.ClientMirror);
		}

		private async void Initialized(ICommunicationMessage e)
		{
			this.Logger.Trace($"Client Initialized: {e.Client.Name}");

			var session = this.sessions.Select(s => s.Value).Single(s => s.User.Id == e.User.Id);

			using (var context = new StorageContext())
			using (var transaction = context.Database.BeginTransaction())
			{
				session.Connected = DateTime.UtcNow;

				context.Sessions.AddOrUpdate(session);
				await context.SaveChangesAsync();
				transaction.Commit();
			}

			this.Comms.Event(SessionEvents.ClientInitialized).ToServer().Emit(e.Client, session);
			//this.Events.Raise(SessionEvents.ClientInitialized, e.Client, session);
		}

		private async Task MonitorSession(Session session, IClient client)
		{
			while (session.IsConnected && this.threads.ContainsKey(session) && !this.threads[session].Item2.Token.IsCancellationRequested)
			{
				await BaseScript.Delay(100);

				if (API.GetPlayerLastMsg(client.Handle.ToString()) <= this.Configuration.ConnectionTimeout.TotalMilliseconds) continue;

				this.Comms.Event(SessionEvents.SessionTimedOut).ToServer().Emit(client, session);
				//await this.Events.RaiseAsync(SessionEvents.SessionTimedOut, client, session);

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
