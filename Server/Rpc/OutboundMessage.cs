using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NFive.Server.Rpc
{
	[PublicAPI]
	public class OutboundMessage
	{
		public Client Target { get; set; }

		public string Event { get; set; }

		public List<string> Payloads { get; set; } = new List<string>();

		public DateTime Created { get; set; } = DateTime.UtcNow;
	}
}
