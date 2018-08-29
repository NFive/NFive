using System.Collections.Generic;
using JetBrains.Annotations;

namespace NFive.Server.Plugins
{
	[PublicAPI]
	public class PluginDefinition
	{
		public string Name { get; set; }

		public string Version { get; set; } = "1.0.0";

		public PluginTypes Type { get; set; } = PluginTypes.Plugin;

		public string Description { get; set; }

		public string Author { get; set; }

		public string License { get; set; }

		public string Website { get; set; }

		public RepositoryDefinition Repository { get; set; } = new RepositoryDefinition();

		public ComponentDefinition Server { get; set; } = new ComponentDefinition();

		public ComponentDefinition Client { get; set; } = new ComponentDefinition();
	}

	[PublicAPI]
	public enum PluginTypes
	{
		Plugin,
		Library,
		LoadingScreen
	}

	[PublicAPI]
	public class RepositoryDefinition
	{
		public string Type { get; set; }

		public string Url { get; set; }
	}

	[PublicAPI]
	public class ComponentDefinition
	{
		public List<string> Main { get; set; } = new List<string>();

		public List<string> Include { get; set; } = new List<string>();

		public Dictionary<string, string> Dependencies { get; set; } = new Dictionary<string, string>();

		public List<ServerPluginDefinition> DependencyNodes { get; set; } = new List<ServerPluginDefinition>();
	}
}
