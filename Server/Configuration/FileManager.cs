using System;
using System.IO;
using CitizenFX.Core.Native;

namespace NFive.Server.Configuration
{
	public static class FileManager
	{
		public static string ResolveResourcePath(string searchFile = "nfive.yml")
		{
			var basePath = AppDomain.CurrentDomain.BaseDirectory;
			var resourcePath = "nfive";

			try
			{
				resourcePath = API.GetCurrentResourceName(); // TODO: https://runtime.fivem.net/doc/natives/#_0x76A9EE1F
			}
			catch (Exception)
			{
				// ignored
			}

			foreach (var path in new[] {
				Path.Combine(basePath),
				Path.Combine(basePath, resourcePath),
				Path.Combine(basePath, "NFive", resourcePath),
				Path.Combine(basePath, "[NFive]", resourcePath),
				Path.Combine(basePath, "[NFive]", resourcePath),
				Path.Combine(basePath, "[NFive]", "NFive", resourcePath),
				Path.Combine(basePath, "resources", resourcePath),
				Path.Combine(basePath, "resources", "NFive", resourcePath),
				Path.Combine(basePath, "resources", "[NFive]", resourcePath),
				Path.Combine(basePath, "resources", "[NFive]", resourcePath),
				Path.Combine(basePath, "resources", "[NFive]", "NFive", resourcePath)
			})
			{
				if (Directory.Exists(path) && File.Exists(Path.Combine(path, searchFile))) return path;
			}

			throw new DirectoryNotFoundException($"Unable to locate resource directory \"{resourcePath}\" at base \"{basePath}\"");
		}
	}
}
