using JetBrains.Annotations;
using Newtonsoft.Json;
using NFive.SDK.Server.Rpc;
using System;
using System.Collections.Generic;

namespace NFive.Server.Rpc
{
	[PublicAPI]
	public class OutboundMessage
	{
		[JsonIgnore]
		public IClient Target { get; set; }

		public string Event { get; set; }

		public List<string> Payloads { get; set; } = new List<string>();

		public DateTime Created { get; set; } = DateTime.UtcNow;

		public DateTime? Sent { get; set; }

		public byte[] Pack() => RpcPacker.Pack(this);
	}
}
