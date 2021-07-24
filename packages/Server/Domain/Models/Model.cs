using System;

namespace Solidarity.Domain.Models
{
	public abstract class Model
	{
		public int Id { get; set; }
		public int CreatorId { get; set; }
		public DateTime Creation { get; set; }
		public int LastModifierId { get; set; }
		public DateTime LastModification { get; set; }
	}
}