using System;
using System.Collections.Generic;
using CitizenFX.Core;
using JetBrains.Annotations;

namespace NFive.Client.Rpc
{
	[PublicAPI]
	public class OutboundMessage
	{
		public int Source { get; set; } = Game.Player.Handle;

		public string Event { get; set; }

		public List<string> Payloads { get; set; } = new List<string>();

		public DateTime Sent { get; set; } = DateTime.UtcNow;
	}
}
