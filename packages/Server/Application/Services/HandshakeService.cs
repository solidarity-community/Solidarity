using Solidarity.Application.Common;
using Solidarity.Core.Application;
using Solidarity.Domain.Exceptions;
using Solidarity.Domain.Models;
using System;
using System.Linq;

namespace Solidarity.Application.Services
{
	public class HandshakeService : Service
	{
		public HandshakeService(IDatabase database, ICryptoClientFactory cryptoClientFactory, ICurrentUserService currentUserService) : base(database, cryptoClientFactory, currentUserService) { }

		public Handshake GetByPhrase(string phrase)
		{
			DeleteInvalid();
			return Database.Handshakes
				.IncludeAll()
				.FirstOrDefault(hs => hs.Phrase == phrase)
				?? throw new EntityNotFoundException("Handshake was not found");
		}

		private void DeleteInvalid()
		{
			Database.Handshakes
				.Where(e => e.Expiration <= DateTime.Now)
				.ToList()
				.ForEach(handshake => Database.Handshakes.Remove(handshake));
			Database.CommitChanges();
		}
	}
}