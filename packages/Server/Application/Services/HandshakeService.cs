namespace Solidarity.Application.Services;

public class HandshakeService : Service
{
	public HandshakeService(IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService) { }

	public async Task<Handshake> GetByPhrase(string phrase)
	{
		await DeleteInvalid();
		return await _database.Handshakes.IncludeAll().FirstOrDefaultAsync(hs => hs.Phrase == phrase)
			?? throw new EntityNotFoundException<Handshake>();
	}

	private async Task DeleteInvalid()
	{
		_database.Handshakes
			.Where(e => e.Expiration <= DateTime.Now)
			.ToList()
			.ForEach(handshake => _database.Handshakes.Remove(handshake));
		await _database.CommitChangesAsync();
	}
}