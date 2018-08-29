using System;
using System.IO;
using NFive.Server.Plugins;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NFive.Server.Configuration
{
	public static class ConfigurationManager
	{
		public static object Load(string name, Type type)
		{
			Deserializer deserializer = new DeserializerBuilder()
				.WithNamingConvention(new CamelCaseNamingConvention())
				//.IgnoreUnmatchedProperties()
				.Build();

			return deserializer.Deserialize(File.ReadAllText(Path.Combine(PluginManager.ConfigurationPath, $"{name}.yml")), type);
		}

		public static T Load<T>(string name)
		{
			return (T)Load(name, typeof(T));
		}
	}
}
