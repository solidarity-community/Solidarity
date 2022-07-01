namespace Solidarity.Application.Abstractions;

[MapToHttpStatusCode(HttpStatusCode.NotFound)]
public class EntityNotFoundException<TEntity> : Exception
{
	public EntityNotFoundException(string message = "Entity was not found") : base(message) { }
}