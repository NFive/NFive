using System.Collections.Generic;
using JetBrains.Annotations;

namespace NFive.Server.Plugins
{
	[PublicAPI]
	public class PluginDefinition
	{
		public string Name { get; set; }

		public string Version { get; set; }

		public string FullName => $"{this.Name}@{this.Version}";

		public string Description { get; set; }

		public string Author { get; set; }

		public string License { get; set; }

		public string Website { get; set; }

		public ComponentDefinition Server { get; set; } = new ComponentDefinition();

		public ComponentDefinition Client { get; set; } = new ComponentDefinition();

		public Dictionary<string, string> Dependencies { get; set; }

		public List<ComponentDefinition> DependencyNodes { get; set; }

		public List<RepositoryDefinition> Repositories { get; set; }
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
