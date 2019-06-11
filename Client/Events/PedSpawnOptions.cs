using CitizenFX.Core;
using CitizenFX.Core.Native;
using JetBrains.Annotations;

namespace NFive.Client.Events
{
	[PublicAPI]
	public class PedSpawnOptions
	{
		private Vector3 position;
		private uint model;
		private readonly dynamic setters;

		public Model Model
		{
			get => new Model((int)this.model);
			set
			{
				this.model = (uint)value.Hash;
				this.setters.setModel(this.model);
			}
		}

		public Vector3 Position
		{
			get => this.position;
			set
			{
				this.position = value;
				this.setters.setPosition(this.position);
			}
		}

		public PedSpawnOptions(float x, float y, float z, uint model, dynamic setters)
		{
			this.position = new Vector3(x, y, z);
			this.model = model;
			this.setters = setters;
		}

		public void Cancel()
		{
			API.CancelEvent();
		}
	}
}
