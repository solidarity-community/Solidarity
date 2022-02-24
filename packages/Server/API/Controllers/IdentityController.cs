namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class IdentityController : ControllerBase
{
	private readonly IdentityService _identityService;

	public IdentityController(IdentityService identityService) => _identityService = identityService;

	[HttpGet("{id}")]
	public ActionResult<string> Get([FromRoute] int id) => Ok(_identityService.Get(id));

	[HttpGet]
	public ActionResult<Identity> GetByAccountId([FromQuery] int? accountId) => Ok(_identityService.GetByAccountId(accountId));

	[HttpPost, HttpPut]
	public ActionResult<Identity> CreateOrUpdate([FromBody] Identity Identity)
		=> Ok(_identityService.CreateOrUpdate(Identity));
}