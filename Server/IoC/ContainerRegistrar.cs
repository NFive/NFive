using Griffin.Container;
using System.Collections.Generic;
using System.Reflection;

namespace NFive.Server.IoC
{
	public class ContainerRegistrar : Griffin.Container.ContainerRegistrar
	{
		public ContainerRegistrar() : base(Lifetime.Transient) { }

		public void RegisterSdkComponents(IEnumerable<Assembly> assemblies)
		{
			foreach (var assembly in assemblies)
			{
				FindTypesUsing<SDK.Core.IoC.ComponentAttribute>(assembly, (attr, type) => RegisterComponent(type, attr.Lifetime == SDK.Core.IoC.Lifetime.Transient ? Lifetime.Transient : Lifetime.Singleton));
			}
		}
	}
}
