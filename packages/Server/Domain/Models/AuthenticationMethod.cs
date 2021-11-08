using System;

namespace Solidarity.Domain.Models
{
	public abstract class AuthenticationMethod : Model
	{
		public int AccountId { get; set; }

		public Account? Account { get; set; }

		public virtual AuthenticationMethodType Type { get; }

		public virtual bool SupportsMultiple => false;

		public virtual int Salt { get; private set; }

		public string Data { get; set; } = null!;

		public bool Authenticate(string data) => Data == GetEncrypted(data);

		public void Encrypt()
		{
			Data = GetEncrypted(Data);
			Salt = SupportsMultiple ? 0 : new Random().Next(1, 99999);
		}

		protected abstract string GetEncrypted(string data);
	}
}