namespace Solidarity.Application.Files;

public sealed class FileService
{
	public const string FileDirectory = "files";
	public const int MaxFileSizeInMB = 5;
	public static readonly string[] AllowedFileExtensions = ["jpg", "png", "gif"];

	private static void EnsureDirectoryExists()
	{
		if (Directory.Exists(FileDirectory) == false)
		{
			Directory.CreateDirectory(FileDirectory);
		}
	}

	private static string GetUri(Guid guid) => $"{FileDirectory}/{guid}";
	private static string GetPath(Guid guid) => Directory.GetFiles(FileDirectory, $"{guid}.*")[0];

	public FileService()
	{
		EnsureDirectoryExists();
	}

	public FileStream Get(Guid guid)
		=> File.OpenRead(GetPath(guid));

	public async Task<Guid> Save(IFormFile file, Guid? existingGuid)
	{
		if (file.Length / 1024 / 1024 > MaxFileSizeInMB)
		{
			throw new FileTooLargeException(MaxFileSizeInMB);
		}

		if (AllowedFileExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
		{
			throw new FileExtensionNotSupportedException(AllowedFileExtensions);
		}

		var guid = existingGuid ?? Guid.NewGuid();
		using var stream = File.Create(GetUri(guid) + Path.GetExtension(file.FileName));
		await file.CopyToAsync(stream);

		return guid;
	}

	public void Delete(Guid guid) => File.Delete(GetPath(guid));
	public void Delete(string guid) => Delete(Guid.Parse(guid));
}