using Microsoft.AspNetCore.Http;
using Solidarity.Application.Common;
using System.Security.Claims;

namespace Solidarity.Infrastructure.Identity
{
	public class CurrentUserService : ICurrentUserService
	{
		public int? Id { get; }
		public bool IsAuthenticated => Id is not null;

		public CurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			var claim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
			var value = claim?.Value;

			if (value is not null)
			{
				Id = IsAuthenticated ? int.Parse(value) : null;
			}
		}
	}
}