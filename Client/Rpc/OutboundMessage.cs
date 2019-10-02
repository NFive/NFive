using CitizenFX.Core;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace NFive.Client.Rpc
{
	[PublicAPI]
	public class OutboundMessage
	{
		public int Source { get; set; } = Game.Player.ServerId;

		public string Event { get; set; }

		public List<string> Payloads { get; set; } = new List<string>();

		public byte[] Pack() => RpcPacker.Pack(this);
	}
}
