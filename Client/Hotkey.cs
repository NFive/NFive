using CitizenFX.Core;
using CitizenFX.Core.Native;
using System;

namespace NFive.Client
{
	public class Hotkey
	{
		public Control Control { get; }

		public JavaScriptCode JavaScriptCode { get; }

		public string ScaleformName { get; }

		public string DisplayName { get; }

		public Hotkey(Control control)
		{
			this.Control = control;
			this.ScaleformName = API.GetControlInstructionalButton(0, (int)this.Control, 0);
			this.JavaScriptCode = KeyMapping.KeyMappings.ContainsKey(this.ScaleformName) ? KeyMapping.KeyMappings[this.ScaleformName] : JavaScriptCode.None; // TODO: JavaScriptCode.Unknown ?
			this.DisplayName = this.JavaScriptCode == JavaScriptCode.None ? this.ScaleformName : Enum.GetName(typeof(JavaScriptCode), this.JavaScriptCode);
		}

		public override string ToString() => this.DisplayName;
	}
}
