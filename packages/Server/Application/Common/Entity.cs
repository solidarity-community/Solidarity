namespace Solidarity.Application.Common;

public abstract class Entity : Model
{
	public int Id { get; set; }
	[JsonIgnore] public bool IsNew => Id is 0;
}