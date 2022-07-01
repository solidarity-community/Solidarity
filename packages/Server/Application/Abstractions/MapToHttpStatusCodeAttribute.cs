namespace Solidarity.Application.Abstractions;

[AttributeUsage(AttributeTargets.Class)]
public class MapToHttpStatusCodeAttribute : Attribute
{
	public HttpStatusCode Code { get; }
	public MapToHttpStatusCodeAttribute(HttpStatusCode code) => Code = code;
}