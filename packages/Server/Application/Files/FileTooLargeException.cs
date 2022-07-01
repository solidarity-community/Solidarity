namespace Solidarity.Application.Files;

public class FileTooLargeException : Exception
{
	public FileTooLargeException(string message = "File is too large") : base(message) { }
	public FileTooLargeException(int maxFileSizeInMB) : base($"Files larger than {maxFileSizeInMB}MB are not supported") { }
}