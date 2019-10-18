using System;
using CitizenFX.Core.Native;
using JetBrains.Annotations;
using NFive.SDK.Core.Controllers;
using YamlDotNet.Serialization;

namespace NFive.Server.Configuration
{
	[PublicAPI]
	public class SessionConfiguration : ControllerConfiguration
	{
		public override string FileName => "session";

		[YamlIgnore]
		public ushort MaxClients => (ushort)API.GetConvarInt("sv_maxclients", 32);

		public TimeSpan ConnectionTimeout { get; set; } = TimeSpan.FromMinutes(1);

		public TimeSpan ReconnectGrace { get; set; } = TimeSpan.FromMinutes(2);
	}
}
