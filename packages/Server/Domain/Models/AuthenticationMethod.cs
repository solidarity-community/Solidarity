namespace Solidarity.Domain.Models
{
	public class AuthenticationMethod : Model
	{
		public int AccountId { get; set; }

		public Account? Account { get; set; }

		private string data = null!;
		public string Data
		{
			get => data;
			set => data = Encrypt(value);
		}

		public bool Authenticate(string data) => Data == Encrypt(data);

		public virtual string Encrypt(string data) => throw new System.Exception("Not implemented");
	}
}