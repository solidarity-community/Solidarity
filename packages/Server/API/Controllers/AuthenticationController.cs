using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solidarity.Application.Services;
using Solidarity.Domain.Models;

namespace Solidarity.Controllers
{
	[ApiController, Route("[controller]"), Authorize]
	public class AuthenticationController : ControllerBase
	{
		private AuthenticationService AuthenticationService { get; }

		public AuthenticationController(AuthenticationService authenticationService) => AuthenticationService = authenticationService;

		[HttpGet("check"), AllowAnonymous]
		public ActionResult<bool> IsAuthenticated() => AuthenticationService.IsAuthenticated();

		[HttpGet]
		public ActionResult<AuthenticationList> GetAll() => AuthenticationService.GetAll();

		[HttpPut("password")]
		public ActionResult UpdatePassword([FromQuery] string newPassword, [FromQuery] string? oldPassword = null)
		{
			AuthenticationService.UpdatePassword(newPassword, oldPassword);
			return Ok();
		}

		[HttpGet("password"), AllowAnonymous]
		public ActionResult<string> PasswordLogin([FromQuery] string username, [FromQuery] string password)
			=> AuthenticationService.PasswordLogin(username, password);
	}
}