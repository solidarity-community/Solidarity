using System;

namespace Solidarity.Domain.Exceptions
{
	public class AccountTakenException : Exception
	{
		public AccountTakenException(string message = "This account has already been taken") : base(message) { }
	}
}