using CitizenFX.Core;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace NFive.Client.Rpc
{
	[PublicAPI]
	public class OutboundMessage
	{
		public int Source { get; set; } = Game.Player.ServerId;

		public string Event { get; set; }

		public List<string> Payloads { get; set; } = new List<string>();

		public DateTime Created { get; set; } = DateTime.UtcNow;

		public DateTime? Sent { get; set; }

		public byte[] Pack() => RpcPacker.Pack(this);
	}
}
