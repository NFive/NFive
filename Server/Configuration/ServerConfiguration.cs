using JetBrains.Annotations;
using NFive.SDK.Server.Configuration;

namespace NFive.Server.Configuration
{
	[PublicAPI]
	public class ServerConfiguration : IServerConfiguration
	{
		public ILocaleConfiguration Locale { get; }

		internal ServerConfiguration(ILocaleConfiguration locale)
		{
			this.Locale = locale;
		}

		
	}
}
