using System.Runtime.CompilerServices;

namespace Solidarity.Application.Common;

[MapToHttpStatusCode(HttpStatusCode.NotFound)]
public sealed class EntityNotFoundException<TEntity>(object? key = null, string? message = null, [CallerArgumentExpression(nameof(key))] string keyName = "") : Exception($"{message ?? "Entity not found"} - {keyName}: {key}")
{
}