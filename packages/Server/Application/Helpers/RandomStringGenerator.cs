using System;
using System.Text;

namespace Solidarity.Application.Helpers
{
	public static class RandomStringGenerator
	{
		public static string Generate(int length)
		{
			var random = new Random();
			var characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz$%^*()-";
			var result = new StringBuilder(length);
			for (var i = 0; i < length; i++)
			{
				result.Append(characters[random.Next(characters.Length)]);
			}
			return result.ToString();
		}
	}
}
