using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Solidarity.Application.Services;
using Solidarity.Domain.Models;

namespace Solidarity.Controllers
{
	[ApiController, Route("[controller]"), Authorize]
	public class AuthenticationController : ControllerBase
	{
		private readonly AuthenticationService authenticationService;

		public AuthenticationController(AuthenticationService authenticationService) => this.authenticationService = authenticationService;

		[HttpGet("check"), AllowAnonymous]
		public ActionResult<bool> IsAuthenticated() => authenticationService.IsAuthenticated();

		[HttpGet]
		public ActionResult<AuthenticationList> GetAll() => authenticationService.GetAll();

		[HttpPut("password")]
		public ActionResult UpdatePassword([FromQuery] string newPassword, [FromQuery] string? oldPassword = null)
		{
			authenticationService.UpdatePassword(newPassword, oldPassword);
			return Ok();
		}

		[HttpGet("password"), AllowAnonymous]
		public ActionResult<string> PasswordLogin([FromQuery] string username, [FromQuery] string password)
			=> authenticationService.PasswordLogin(username, password);
	}
}