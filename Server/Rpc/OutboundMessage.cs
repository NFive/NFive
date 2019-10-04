using JetBrains.Annotations;
using Newtonsoft.Json;
using NFive.SDK.Server.Communications;
using System;
using System.Collections.Generic;

namespace NFive.Server.Rpc
{
	[PublicAPI]
	public class OutboundMessage
	{
		public Guid Id { get; set; }

		[JsonIgnore]
		public IClient Target { get; set; }

		public string Event { get; set; }

		public List<string> Payloads { get; set; } = new List<string>();

		public byte[] Pack() => RpcPacker.Pack(this);
	}
}
