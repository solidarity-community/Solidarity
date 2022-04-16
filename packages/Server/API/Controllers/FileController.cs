namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class FileController : ControllerBase
{
	private readonly FileService _fileService;

	public FileController(FileService fileService) => _fileService = fileService;

	[HttpGet("max-size")]
	public ActionResult<int> GetMaxFileSizeInMB() => Ok(FileService.MaxFileSizeInMB);

	[HttpGet("allowed-extensions")]
	public ActionResult<string[]> AllowedFileExtensions() => Ok(FileService.AllowedFileExtensions);

	[HttpGet("{guid}"), AllowAnonymous]
	public FileStreamResult GetFile([FromRoute] Guid guid) => File(_fileService.Get(guid), "application/octet-stream");

	[HttpPost, HttpPut]
	public ActionResult<string> CreateOrUpdate([FromForm] IFormFile file)
		=> Ok(_fileService.Save(file, null));

	[HttpDelete("{guid}")]
	public ActionResult Delete([FromRoute] Guid guid)
	{
		_fileService.Delete(guid);
		return Ok();
	}
}