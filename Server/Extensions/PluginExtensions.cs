using NFive.SDK.Core.Plugins;
using System;
using System.IO;
using System.Reflection;

namespace NFive.Server.Extensions
{
	public static class PluginExtensions
	{
		public static Assembly GetIncludeFileAssembly(this Plugin plugin, string includeName)
		{
			var includeFile = Path.Combine("plugins", plugin.Name.Vendor, plugin.Name.Project, $"{includeName}.net.dll");
			if (!File.Exists(includeFile)) throw new FileNotFoundException(includeFile);

			return AppDomain.CurrentDomain.Load(File.ReadAllBytes(includeFile));
		}

		public static Assembly GetMainFileAssembly(this Plugin plugin, string mainName)
		{
			var mainFile = Path.Combine("plugins", plugin.Name.Vendor, plugin.Name.Project, $"{mainName}.net.dll");
			if (!File.Exists(mainFile)) throw new FileNotFoundException(mainFile);

			return Assembly.LoadFrom(mainFile);
		}
	}
}
