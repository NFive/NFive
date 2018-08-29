using System;
using System.Collections.Generic;
using CitizenFX.Core;

namespace NFive.Client.Rpc
{
	public class OutboundMessage
	{
		public int Source { get; set; } = Game.Player.ServerId;

		public string Event { get; set; }

		public List<string> Payloads { get; set; } = new List<string>();

		public DateTime Sent { get; set; } = DateTime.UtcNow;
	}
}
