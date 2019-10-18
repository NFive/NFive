using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace NFive.Client.Rpc
{
	[PublicAPI]
	public class InboundMessage
	{
		public Guid Id { get; set; }

		public string Event { get; set; }

		public List<string> Payloads { get; set; }

		public static InboundMessage From(byte[] data) => RpcPacker.Unpack(data);
	}
}
