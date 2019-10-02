using JetBrains.Annotations;

namespace NFive.Server.Controllers
{
	[PublicAPI]
	public static class FiveMClientEvents
	{
		public const string ResourceStart = "onClientResourceStart";
		public const string ResourceStop = "onClientResourceStop";
		public const string PopulationPedCreating = "populationPedCreating";
	}
}
