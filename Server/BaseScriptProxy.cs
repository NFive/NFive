using CitizenFX.Core;
using NFive.SDK.Server;

namespace NFive.Server
{
	internal class BaseScriptProxy : IBaseScriptProxy
	{
		public EventHandlerDictionary EventHandlers { get; }

		public ExportDictionary Exports { get; }

		public PlayerList Players { get; }

		public BaseScriptProxy(EventHandlerDictionary eventHandlers, ExportDictionary exports, PlayerList players)
		{
			this.EventHandlers = eventHandlers;
			this.Exports = exports;
			this.Players = players;
		}
	}
}
