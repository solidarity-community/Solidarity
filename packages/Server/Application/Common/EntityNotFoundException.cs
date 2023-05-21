namespace Solidarity.Application.Common;

[MapToHttpStatusCode(HttpStatusCode.NotFound)]
public class EntityNotFoundException<TEntity> : Exception
{
	public EntityNotFoundException(string message = "Entity was not found") : base(message) { }
}