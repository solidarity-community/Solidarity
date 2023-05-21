namespace Solidarity.Application.Common;

[AttributeUsage(AttributeTargets.Class)]
public class MapToHttpStatusCodeAttribute : Attribute
{
	public HttpStatusCode Code { get; }
	public MapToHttpStatusCodeAttribute(HttpStatusCode code) => Code = code;
}