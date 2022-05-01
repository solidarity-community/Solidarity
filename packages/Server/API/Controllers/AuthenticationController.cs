namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class AuthenticationController : ControllerBase
{
	private readonly AuthenticationService _authenticationService;

	public AuthenticationController(AuthenticationService authenticationService) => _authenticationService = authenticationService;

	[HttpGet("check"), AllowAnonymous]
	public async Task<ActionResult<bool>> IsAuthenticated() => Ok(await _authenticationService.IsAuthenticated());

	[HttpGet]
	public async Task<ActionResult<Dictionary<AuthenticationMethodType, bool>>> GetAll() => Ok(await _authenticationService.GetAll());

	[HttpPut("password")]
	public async Task<ActionResult> UpdatePassword([FromQuery] string newPassword, [FromQuery] string? oldPassword = null)
	{
		await _authenticationService.UpdatePassword(newPassword, oldPassword);
		return Ok();
	}

	[HttpGet("password"), AllowAnonymous]
	public async Task<ActionResult<string>> PasswordLogin([FromQuery] string username, [FromQuery] string password)
		=> await _authenticationService.PasswordLogin(username, password);
}