namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class AccountController : ControllerBase
{
	private readonly AccountService _accountService;

	public AccountController(AccountService accountService) => _accountService = accountService;

	[HttpGet]
	public async Task<ActionResult<Account>> Get() => Ok(await _accountService.GetWithoutAuthentication(null));

	[HttpPost, AllowAnonymous]
	public async Task<ActionResult<string>> Create([FromBody] Account account)
		=> Ok(await _accountService.CreateAndIssueToken(account));

	[HttpPut]
	public async Task<ActionResult<Account>> Update([FromBody] Account account)
		=> Ok(await _accountService.Update(account));

	[HttpGet("{id}/reset"), AllowAnonymous]
	public async Task<ActionResult<string>> Reset([FromRoute] int id) => Ok(await _accountService.Reset(id));

	[HttpGet("recover"), AllowAnonymous]
	public async Task<ActionResult<string>> Recover([FromQuery] string phrase) => Ok(await _accountService.Recover(phrase));
}