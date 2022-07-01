namespace Solidarity.Application.Abstractions;

[TransientService]
public interface ICurrentUserService
{
	int? Id { get; }
	bool IsAuthenticated { get; }
}