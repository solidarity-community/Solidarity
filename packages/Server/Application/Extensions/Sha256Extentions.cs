namespace Solidarity.Application.Extensions;

public static class Sha256Extentions
{
	public static string GetSha256Hash(this string value)
	{
		if (value == null)
		{
			return null;
		}
		HashAlgorithm algorithm = SHA256.Create();
		var hashedArr = algorithm.ComputeHash(Encoding.UTF8.GetBytes(value));
		var sb = new StringBuilder();
		foreach (var b in hashedArr)
		{
			sb.Append(b.ToString("X2"));
		}
		return sb.ToString();
	}
}