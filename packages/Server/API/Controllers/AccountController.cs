namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class AccountController : ControllerBase
{
	private readonly AccountService _accountService;

	public AccountController(AccountService accountService) => _accountService = accountService;

	[HttpGet]
	public ActionResult<Account> Get() => Ok(_accountService.GetWithoutAuthentication(null));

	[HttpPost, AllowAnonymous]
	public ActionResult<string> Create([FromBody] Account account)
		=> Ok(_accountService.CreateAndIssueToken(account));

	[HttpPut]
	public ActionResult<Account> Update([FromBody] Account account)
		=> Ok(_accountService.Update(account));

	[HttpGet("{id}/reset"), AllowAnonymous]
	public ActionResult<string> Reset([FromRoute] int id) => Ok(_accountService.Reset(id));

	[HttpGet("recover"), AllowAnonymous]
	public ActionResult<string> Recover([FromQuery] string phrase) => Ok(_accountService.Recover(phrase));
}