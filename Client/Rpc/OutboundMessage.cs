using System;
using System.Collections.Generic;
using CitizenFX.Core;
using JetBrains.Annotations;

namespace NFive.Client.Rpc
{
	[PublicAPI]
	public class OutboundMessage
	{
		public Guid Id { get; set; }

		public int Source { get; set; } = Game.Player.ServerId;

		public string Event { get; set; }

		public List<string> Payloads { get; set; } = new List<string>();

		public byte[] Pack() => RpcPacker.Pack(this);
	}
}
