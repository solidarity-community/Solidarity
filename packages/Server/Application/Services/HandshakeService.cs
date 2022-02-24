namespace Solidarity.Application.Services;

public class HandshakeService : Service
{
	public HandshakeService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService) { }

	public Handshake GetByPhrase(string phrase)
	{
		DeleteInvalid();
		return _database.Handshakes
			.IncludeAll()
			.FirstOrDefault(hs => hs.Phrase == phrase)
			?? throw new EntityNotFoundException<Handshake>();
	}

	private void DeleteInvalid()
	{
		_database.Handshakes
			.Where(e => e.Expiration <= DateTime.Now)
			.ToList()
			.ForEach(handshake => _database.Handshakes.Remove(handshake));
		_database.CommitChanges();
	}
}