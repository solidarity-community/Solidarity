using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace Solidarity.Domain.Models
{
	public class Account : Model
	{
		[MaxLength(50), Required(ErrorMessage = "Username cannot be null")]
		private string username = null!;
		public string Username { get => username; set => username = value.ToLower(); }

		[Required(ErrorMessage = "Public-key cannot be null")]
		public string PublicKey { get; set; } = null!;

		public List<AuthenticationMethod> AuthenticationMethods { get; set; } = new();

		public List<Campaign> Campaigns { get; set; } = new();

		public List<Vote> Votes { get; set; } = new();
	}
}