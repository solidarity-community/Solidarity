namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class AuthenticationController : ControllerBase
{
	private readonly AuthenticationService _authenticationService;

	public AuthenticationController(AuthenticationService authenticationService) => _authenticationService = authenticationService;

	[HttpGet("check"), AllowAnonymous]
	public ActionResult<bool> IsAuthenticated() => _authenticationService.IsAuthenticated();

	[HttpGet]
	public ActionResult<Dictionary<AuthenticationMethodType, bool>> GetAll() => _authenticationService.GetAll();

	[HttpPut("password")]
	public ActionResult UpdatePassword([FromQuery] string newPassword, [FromQuery] string? oldPassword = null)
	{
		_authenticationService.UpdatePassword(newPassword, oldPassword);
		return Ok();
	}

	[HttpGet("password"), AllowAnonymous]
	public ActionResult<string> PasswordLogin([FromQuery] string username, [FromQuery] string password)
		=> _authenticationService.PasswordLogin(username, password);
}