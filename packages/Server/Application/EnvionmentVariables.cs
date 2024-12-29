namespace Solidarity.Application;

public static class EnvironmentVariables
{
	public static readonly string JWT_KEY = Environment.GetEnvironmentVariable("JWT_KEY") ?? "75pQjmReLS7FRTYZg7ehvb2NE5ZtEkdMdw7h7CKgLh4S8T45jUS9jbuUqLeeUdcP";
	public static readonly string? DATABASE_SERVER = Environment.GetEnvironmentVariable("DATABASE_SERVER");
	public static readonly string? DATABASE_USER = Environment.GetEnvironmentVariable("DATABASE_USER");
	public static readonly string? DATABASE_PASSWORD = Environment.GetEnvironmentVariable("DATABASE_PASSWORD");
	public static readonly string[] PAYMENT_METHODS = [.. Environment.GetEnvironmentVariable("PAYMENT_METHODS")?.Split(',').Select(x => x.Trim()) ?? []];
	public static PaymentMethodEnvironmentVariables GetPaymentMethod(string name) => new(name);
}

public sealed class PaymentMethodEnvironmentVariables(string identifier)
{
	public string SERVER => Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{identifier}_SERVER") ?? throw new InvalidOperationException($"PAYMENT_METHOD_{identifier}_SERVER environment variable is not set");
	public string USERNAME => Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{identifier}_USERNAME") ?? throw new InvalidOperationException($"PAYMENT_METHOD_{identifier}_USERNAME environment variable is not set");
	public string PASSWORD => Environment.GetEnvironmentVariable($"PAYMENT_METHOD_{identifier}_PASSWORD") ?? throw new InvalidOperationException($"PAYMENT_METHOD_{identifier}_PASSWORD environment variable is not set");
	public (string server, string username, string password) Deconstruct() => (SERVER, USERNAME, PASSWORD);
}