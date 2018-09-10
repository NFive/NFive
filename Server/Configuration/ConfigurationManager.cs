using System;
using System.IO;
using NFive.SDK.Plugins.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NFive.Server.Configuration
{
	public static class ConfigurationManager
	{
		public static object Load(string path, Type type)
		{
			path = Path.Combine("config", path);

			if (!File.Exists(path)) throw new FileNotFoundException("Unable to find configuration file", path);

			Deserializer deserializer = new DeserializerBuilder()
				.WithNamingConvention(new UnderscoredNamingConvention())
				//.IgnoreUnmatchedProperties()
				.Build();

			return deserializer.Deserialize(File.ReadAllText(path), type);
		}

		public static object Load(Name name, string file, Type type)
		{
			return Load(Path.Combine(name.Vendor, name.Project, $"{file}.yml"), type);
		}
		
		public static T Load<T>(string name)
		{
			return (T)Load(name, typeof(T));
		}
	}
}
