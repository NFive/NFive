using System;
using System.Data;
using System.IO;
using System.Linq;
using SemVer;
using Version = SemVer.Version;

namespace NFive.Server.Plugins
{
	public class ServerPluginDefinition
	{
		public string Location { get; set; }

		public PluginDefinition Definition { get; set; }

		public PluginDefinitionGraph ResolveDependencies()
		{
			var graph = new PluginDefinitionGraph();

			foreach (var dependency in this.Definition.Server.Dependencies)
			{
				var pluginPath = Path.Combine(Environment.CurrentDirectory, PluginManager.PluginPath, dependency.Key);
				if (!Directory.Exists(pluginPath)) throw new DirectoryNotFoundException($"Unable to find the plugin directory \"{pluginPath}\"");

				var pluginDefinition = PluginManager.LoadDefinition(Path.Combine(pluginPath, PluginManager.DefinitionFile));

				graph.Definitions.Add(pluginDefinition);
			}

			foreach (var plugin in graph.Definitions)
			{
				if (plugin.Definition.Server.Dependencies == null) continue;
				foreach (var dependency in plugin.Definition.Server.Dependencies)
				{
					var dependencyPlugin = graph.Definitions.FirstOrDefault(p => p.Definition.Name == dependency.Key);
					if (dependencyPlugin == null) throw new Exception($"Unable to find dependency {dependency.Key}@{dependency.Value} required by {plugin.Definition.Name}@{plugin.Definition.Version}"); // TODO: DependencyException

					var version = new Version(dependencyPlugin.Definition.Version);
					var required = new Range(dependency.Value);

					if (!required.IsSatisfied(version)) throw new VersionNotFoundException($"{plugin.Definition.Name}@{plugin.Definition.Version} requires {dependencyPlugin.Definition.Name}@{dependency.Value} but {dependencyPlugin.Definition.Name}@{dependencyPlugin.Definition.Version} was found");

					plugin.Definition.Server.DependencyNodes.Add(dependencyPlugin);
				}
			}

			// Perform a topological sort
			graph.Sort();

			return graph;
		}
	}
}
