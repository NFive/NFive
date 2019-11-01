using System;
using JetBrains.Annotations;
using NFive.SDK.Client.Interface;

namespace NFive.Client.Interface
{
	[PublicAPI]
	public class OverlayManager : IOverlayManager
	{
		private readonly INuiManager nui;

		public string Plugin { get; }

		public OverlayManager(string plugin, INuiManager nui)
		{
			this.Plugin = plugin;
			this.nui = nui;
		}

		public void Focus(bool hasFocus, bool showCursor)
		{
			this.nui.Focus(hasFocus, showCursor);
		}

		public void Emit(string @event, object data = null)
		{
			this.nui.Emit(new
			{
				plugin = this.Plugin,
				@event,
				data
			});
		}

		public void On(string @event, Action action)
		{
			this.nui.On($"{this.Plugin}/{@event}", action);
		}

		public void On<T>(string @event, Action<T> action)
		{
			this.nui.On($"{this.Plugin}/{@event}", action);
		}

		public void On<TReturn>(string @event, Func<TReturn> action)
		{
			this.nui.On($"{this.Plugin}/{@event}", action);
		}

		public void On<T, TReturn>(string @event, Func<T, TReturn> action)
		{
			this.nui.On($"{this.Plugin}/{@event}", action);
		}
	}
}
