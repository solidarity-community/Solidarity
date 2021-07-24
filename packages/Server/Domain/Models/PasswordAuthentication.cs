using Solidarity.Application.Extensions;

namespace Solidarity.Domain.Models
{
	public class PasswordAuthentication : AuthenticationMethod
	{
		public override string Encrypt(string data) => data.GetSha256Hash();
	}
}