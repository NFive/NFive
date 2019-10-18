using System.Collections.Generic;
using System.Reflection;
using Griffin.Container;

namespace NFive.Server.IoC
{
	public class ContainerRegistrar : Griffin.Container.ContainerRegistrar
	{
		public ContainerRegistrar() : base(Lifetime.Transient) { }

		public void RegisterPluginComponents(IEnumerable<Assembly> assemblies)
		{
			foreach (var assembly in assemblies)
			{
				FindTypesUsing<SDK.Server.IoC.ComponentAttribute>(assembly, (attr, type) => RegisterComponent(type, attr.Lifetime == SDK.Server.IoC.Lifetime.Transient ? Lifetime.Transient : Lifetime.Singleton));
			}
		}
	}
}
