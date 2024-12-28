namespace Solidarity.Api;

using Options = System.Text.Json.JsonSerializerOptions;

public static class JsonSerializerOptions
{
	public static readonly Options Default = Get();
	public static Options Get(Options? options = null)
	{
		options ??= new Options(JsonSerializerDefaults.Web);
		options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
		options.Converters.Add(new DateTimeConverter());
		options.Converters.Add(new NetTopologySuite.IO.Converters.GeoJsonConverterFactory(
			new GeometryFactoryEx(precisionModel: new PrecisionModel(), srid: 4326) { OrientationOfExteriorRing = LinearRingOrientation.CCW }
		));
		options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		options.PropertyNameCaseInsensitive = true;
		return options;
	}
}