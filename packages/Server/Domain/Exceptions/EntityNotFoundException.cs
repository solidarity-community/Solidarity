namespace Solidarity.Domain.Exceptions;

public class EntityNotFoundException<TEntity> : Exception
{
	public EntityNotFoundException(string message = $"{nameof(TEntity)} was not found") : base(message) { }
}