namespace Solidarity.Application.Files;

public sealed class FileModule : Module
{
	public override void ConfigureServices(IServiceCollection services) => services.AddTransient<FileService>();

	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/api/file/max-size", () => FileService.MaxFileSizeInMB);
		endpoints.MapGet("/api/file/allowed-extensions", () => FileService.AllowedFileExtensions);
		endpoints.MapGet("/api/file/{guid}", [AllowAnonymous] (FileService fileService, Guid guid) => Results.File(fileService.Get(guid), "application/octet-stream"));
		endpoints.MapPost("/api/file", async (FileService fileService, HttpRequest request) =>
		{
			var file = request.Form.Files.GetFile("file") ?? throw new InvalidOperationException("No file was provided");
			return await fileService.Save(file, null);
		});
		endpoints.MapDelete("/api/file/{guid}", (FileService fileService, Guid guid) => fileService.Delete(guid));
	}
}