using System;

namespace Solidarity.Domain.Exceptions
{
	public class EntityNotFoundException<TEntity> : Exception
	{
		// Convert "Entity" to $"{nameof(TEntity)}" in C# 10
		public EntityNotFoundException(string message = "Entity was not found") : base(message) { }
	}
}