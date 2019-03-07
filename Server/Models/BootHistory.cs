using System;
using NFive.SDK.Core.Helpers;
using System.ComponentModel.DataAnnotations;

namespace NFive.Server.Models
{
	public class BootHistory
	{
		[Key]
		[Required]
		public Guid Id { get; set; }

		[Required]
		public DateTime Created { get; set; }

		public DateTime LastActive { get; set; }

		public BootHistory()
		{
			this.Id = GuidGenerator.GenerateTimeBasedGuid();
			this.Created = DateTime.UtcNow;
			this.LastActive = DateTime.UtcNow;
		}
	}
}
