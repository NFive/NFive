using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NFive.Server.Rpc
{
	[PublicAPI]
	public class InboundMessage
	{
		public int Source { get; set; }

		public string Event { get; set; }

		public List<string> Payloads { get; set; }

		public DateTime Created { get; set; }

		public DateTime Sent { get; set; }

		public DateTime Received { get; set; } = DateTime.UtcNow;
	}
}
