namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class IdentityController : ControllerBase
{
	private readonly IdentityService _identityService;

	public IdentityController(IdentityService identityService) => _identityService = identityService;

	[HttpGet("{id}")]
	public async Task<ActionResult<string>> Get([FromRoute] int id) => Ok(await _identityService.Get(id));

	[HttpGet]
	public async Task<ActionResult<Identity>> GetByAccountId([FromQuery] int? accountId) => Ok(await _identityService.GetByAccountId(accountId));

	[HttpPost, HttpPut]
	public async Task<ActionResult<Identity>> CreateOrUpdate([FromBody] Identity Identity)
		=> Ok(await _identityService.CreateOrUpdate(Identity));
}