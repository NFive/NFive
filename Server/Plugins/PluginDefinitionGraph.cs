using System.Collections.Generic;
using CitizenFX.Core;

namespace NFive.Server.Plugins
{
	public class PluginDefinitionGraph
	{
		public List<ServerPluginDefinition> Definitions { get; private set; } = new List<ServerPluginDefinition>();

		public void Sort()
		{
			var results = new List<ServerPluginDefinition>();

			Visit(this.Definitions, results, new List<ServerPluginDefinition>(), new List<ServerPluginDefinition>());

			this.Definitions = results;
		}

		private static void Visit(IEnumerable<ServerPluginDefinition> graph, ICollection<ServerPluginDefinition> results, ICollection<ServerPluginDefinition> dead, ICollection<ServerPluginDefinition> pending)
		{
			foreach (var n in graph)
			{
				if (dead.Contains(n)) continue;

				if (!pending.Contains(n))
				{
					pending.Add(n);
				}
				else
				{
					Debug.WriteLine($"Cycle detected (node Data={n.Definition.Name})");
					return;
				}

				Visit(n.Definition.Server.DependencyNodes, results, dead, pending);

				if (pending.Contains(n)) pending.Remove(n);

				dead.Add(n);

				results.Add(n);
			}
		}
	}
}
