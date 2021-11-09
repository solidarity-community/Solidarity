namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class IdentityController : ControllerBase
{
	private readonly IdentityService identityService;

	public IdentityController(IdentityService campainService) => identityService = campainService;

	[HttpGet("{id}")]
	public ActionResult<string> Get([FromRoute] int id) => Ok(identityService.Get(id));

	[HttpGet]
	public ActionResult<Identity> GetByAccountId([FromQuery] int? accountId) => Ok(identityService.GetByAccountId(accountId));

	[HttpPost, HttpPut]
	public ActionResult<Identity> CreateOrUpdate([FromBody] Identity Identity)
		=> Ok(identityService.CreateOrUpdate(Identity));
}