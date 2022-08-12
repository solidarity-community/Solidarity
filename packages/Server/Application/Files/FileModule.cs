namespace Solidarity.Application.Files;

public class FileModule : Module
{
	public override void ConfigureEndpoints(IEndpointRouteBuilder endpoints)
	{
		endpoints.MapGet("/file/max-size", () => FileService.MaxFileSizeInMB);

		endpoints.MapGet("/file/allowed-extensions", () => FileService.AllowedFileExtensions);

		endpoints.MapGet("/file/{guid}", [AllowAnonymous] (FileService fileService, Guid guid) => Results.File(fileService.Get(guid), "application/octet-stream"));

		endpoints.MapPost("/file", async (FileService fileService, HttpRequest request) =>
		{
			var file = request.Form.Files.GetFile("file") ?? throw new InvalidOperationException("No file was provided");
			return await fileService.Save(file, null);
		});

		endpoints.MapDelete("/file/{guid}", (FileService fileService, Guid guid) => fileService.Delete(guid));
	}
}