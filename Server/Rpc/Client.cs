using CitizenFX.Core.Native;
using NFive.SDK.Server.Rpc;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace NFive.Server.Rpc
{
	public class Client : IClient
	{
		public int Handle { get; }

		public string Name { get; }

		public string EndPoint { get; }

		public string License { get; }

		public long? SteamId { get; }

		public ulong? DiscordId { get; }

		public ulong? LiveId { get; }

		public ulong? XboxLiveId { get; }

		public int Ping
		{
			get
			{
				if (this.Handle > ushort.MaxValue) return -1;
				return API.GetPlayerPing(this.Handle.ToString());
			}
		}

		public Client(string handle) : this(int.Parse(handle)) { }

		public Client(int handle)
		{
			this.Handle = handle;
			this.Name = API.GetPlayerName(this.Handle.ToString());

			var ids = GetIdentifiers();

			this.EndPoint = ids["ip"];
			this.License = ids["license"];
			this.SteamId = ids.ContainsKey("steam") && !string.IsNullOrEmpty(ids["steam"]) ? long.Parse(ids["steam"], NumberStyles.HexNumber) : default(long?);
			this.DiscordId = ids.ContainsKey("discord") && !string.IsNullOrEmpty(ids["discord"]) ? ulong.Parse(ids["discord"]) : default(ulong?);
			this.LiveId = ids.ContainsKey("live") && !string.IsNullOrEmpty(ids["live"]) ? ulong.Parse(ids["live"]) : default(ulong?);
			this.XboxLiveId = ids.ContainsKey("xbl") && !string.IsNullOrEmpty(ids["xbl"]) ? ulong.Parse(ids["xbl"]) : default(ulong?);
		}

		protected Dictionary<string, string> GetIdentifiers()
		{
			var results = new Dictionary<string, string>();
			var count = API.GetNumPlayerIdentifiers(this.Handle.ToString());

			for (var i = 0; i < count; ++i)
			{
				var id = API.GetPlayerIdentifier(this.Handle.ToString(), i);
				var parts = id.Split(new[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

				results.Add(parts[0], parts[1]);
			}

			return results;
		}

		public IRpcTrigger Event(string @event) => RpcManager.Event(@event);
	}
}
