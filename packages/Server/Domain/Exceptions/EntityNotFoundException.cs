namespace Solidarity.Domain.Exceptions;

public class EntityNotFoundException<TEntity> : Exception
{
	public EntityNotFoundException(string message = "Entity was not found") : base(message) { }
}