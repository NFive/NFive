using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NFive.Server.Plugins
{
	public static class PluginManager
	{
		public const string DefinitionFile = "plugin.yml";
		public const string PluginPath = "Plugins";
		public const string ConfigurationPath = "config";

		public static ServerPluginDefinition LoadDefinition(string file = DefinitionFile)
		{
			if (string.IsNullOrEmpty(file)) throw new ArgumentNullException(nameof(file));
			if (!File.Exists(file)) throw new FileNotFoundException($"Unable to find the plugin definition file \"{file}\"");

			Deserializer deserializer = new DeserializerBuilder()
				.WithNamingConvention(new CamelCaseNamingConvention())
				//.IgnoreUnmatchedProperties()
				.Build();

			var definition = deserializer.Deserialize<PluginDefinition>(File.ReadAllText(file));

			return new ServerPluginDefinition
			{
				Location = Path.GetDirectoryName(file),
				Definition = definition
			};
		}
	}
}
