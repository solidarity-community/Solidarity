﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Solidarity.Domain.Models
{
	public struct IdentityRoles
	{
		public const string Admin = "Admin";
		public const string Member = "Member";
	}

	public class Identity : Model
	{
		public int AccountId { get; set; }

		public Account Account { get; set; } = null!;

		[MaxLength(50)]
		public string FirstName { get; set; } = null!;

		[MaxLength(50)]
		public string LastName { get; set; } = null!;

		public DateTime? BirthDate { get; set; }
	}
}