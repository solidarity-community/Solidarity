using Microsoft.IdentityModel.Tokens;
using Solidarity.Domain.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Solidarity.Application.Helpers
{
	public static class Token
	{
		private static string Issue(TimeSpan expiration, ICollection<(string key, string value)> pairs)
		{
			var claims = new List<Claim>()
			{
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
			};

			claims.AddRange(pairs.Where(pair => pair.value != null).Select(pair => new Claim(pair.key, pair.value)));

			var secretKey = Program.Configuration!["JWT_KEY"];
			var issuer = "Solidarity";
			var audience = "Solidarity";

			var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)), SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				issuer: issuer,
				audience: audience,
				claims: claims.ToArray(),
				expires: DateTime.Now.Add(expiration),
				signingCredentials: credentials);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public static string IssueAccountAccess(this Account account, TimeSpan expiration)
		{
			return Issue(expiration,
				new[] {
					(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
					(JwtRegisteredClaimNames.NameId, account.Username) }
			);
		}

	}
}
