namespace Solidarity.Application.Common;

public abstract class Model
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "IDE1006", Justification = "This is an indicator for the client to be able to identify the type of the model")]
	public string __typeName__ => GetType().Name;
}