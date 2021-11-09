using Solidarity.Application.Common;
using Solidarity.Core.Application;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Models;
namespace Solidarity.Application.Services;

public class HandshakeService : Service
{
	public HandshakeService(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService) : base(database, cryptoClientFactory, currentUserService) { }

	public Handshake GetByPhrase(string phrase)
	{
		DeleteInvalid();
		return database.Handshakes
			.IncludeAll()
			.FirstOrDefault(hs => hs.Phrase == phrase)
			?? throw new EntityNotFoundException<Handshake>();
	}

	private void DeleteInvalid()
	{
		database.Handshakes
			.Where(e => e.Expiration <= DateTime.Now)
			.ToList()
			.ForEach(handshake => database.Handshakes.Remove(handshake));
		database.CommitChanges();
	}
}