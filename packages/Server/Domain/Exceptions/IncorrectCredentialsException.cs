using System;

namespace Solidarity.Domain.Exceptions
{
	public class IncorrectCredentialsException : Exception
	{
		public IncorrectCredentialsException(string message = "Incorrect Credentials") : base(message) { }
	}
}