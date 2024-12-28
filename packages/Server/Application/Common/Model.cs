namespace Solidarity.Application.Common;

public static class TypeExtensions
{
	public static string GetNameExcludingNamespace(this Type type)
	{
		return type switch
		{
			{ FullName: null } => type.Name,
			{ Namespace: null } => type.FullName.Replace('+', '.'),
			_ => type.FullName[(type.Namespace.Length + 1)..].Replace('+', '.')
		};
	}
}

public abstract class Model
{
	[JsonPropertyName("@type")] public string TypeName => GetType().GetNameExcludingNamespace();

	[NotMapped] public ICollection<ValidationResult>? ValidationErrors => this.GetValidationErrors();
	public virtual void Validate() => this.ValidateObject();
}