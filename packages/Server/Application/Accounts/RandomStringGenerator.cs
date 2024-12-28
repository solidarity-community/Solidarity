namespace Solidarity.Application.Accounts;

public static class RandomStringGenerator
{
	private const string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz$%^*()-";

	public static string Generate(int length)
	{
		var random = new Random();
		return Enumerable.Range(0, length)
			.ToList()
			.Aggregate(
				new StringBuilder(length),
				(stringBuilder, index) => stringBuilder.Append(characters[random.Next(characters.Length)])
			).ToString();
	}
}