namespace Solidarity.Application.Common;

[AttributeUsage(AttributeTargets.Class)]
public sealed class MapToHttpStatusCodeAttribute(HttpStatusCode code) : Attribute
{
	public HttpStatusCode Code { get; } = code;
}