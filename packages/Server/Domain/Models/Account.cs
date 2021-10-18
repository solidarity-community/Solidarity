﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Solidarity.Domain.Models
{
	public class Account : Model
	{
		[MaxLength(50), Required(ErrorMessage = "Username cannot be null")]
		public string Username { get; set; } = null!;

		[Required(ErrorMessage = "Public-key cannot be null")]
		public string PublicKey { get; set; } = null!;

		public PasswordAuthentication? PasswordAuthentication { get; set; }

		public Identity? Identity { get; set; }

		public List<Campaign> Campaigns { get; set; } = new();

		public List<Vote> Votes { get; set; } = new();

		public List<AuthenticationMethod> GetAuthentications()
		{
			var list = new List<AuthenticationMethod>();
			if (PasswordAuthentication != null)
			{
				list.Add(PasswordAuthentication);
			}
			return list;
		}

		public AuthenticationList GetAuthenticationList()
		{
			return new AuthenticationList { Password = PasswordAuthentication != null };
		}

		public T? GetAuthentication<T>() where T : AuthenticationMethod
		{
			return typeof(T) == typeof(PasswordAuthentication) ? PasswordAuthentication as T : null;
		}

		public void SetAuthenticationMethod<T>(T model) where T : AuthenticationMethod
		{
			if (model is PasswordAuthentication)
			{
				PasswordAuthentication = model as PasswordAuthentication;
			}
		}

		public void DeleteAuthenticationMethods()
		{
			PasswordAuthentication = null;
		}
	}
}