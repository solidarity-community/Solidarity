namespace Solidarity.Application.Files;

public sealed class FileExtensionNotSupportedException : Exception
{
	public FileExtensionNotSupportedException(string message = "File extension is not supported") : base(message) { }
	public FileExtensionNotSupportedException(string[] supportedExtensions) : base($"Supported file extensions are {string.Join(", ", supportedExtensions)}.") { }
}