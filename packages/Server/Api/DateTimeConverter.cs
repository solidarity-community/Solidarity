namespace Solidarity.Api;

public sealed class DateTimeConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
		=> reader.GetDateTime().ToUniversalTime();

	public override void Write(Utf8JsonWriter writer, DateTime value, System.Text.Json.JsonSerializerOptions options)
		=> writer.WriteStringValue(value.ToUniversalTime());
}