namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class CampaignController : ControllerBase
{
	private readonly CampaignService _campaignService;

	public CampaignController(CampaignService campaignService) => _campaignService = campaignService;

	[HttpGet, AllowAnonymous]
	public async Task<ActionResult<IEnumerable<Campaign>>> GetAll() => Ok(await _campaignService.GetAll());

	[HttpGet("{id}"), AllowAnonymous]
	public async Task<ActionResult<string>> Get([FromRoute] int id) => Ok(await _campaignService.Get(id));

	[HttpGet("{id}/balance"), AllowAnonymous]
	public async Task<ActionResult<decimal>> GetBalance([FromRoute] int id) => Ok(await _campaignService.GetBalance(id));

	[HttpPost]
	public async Task<ActionResult<Campaign>> Create([FromBody] Campaign campaign)
		=> Ok(await _campaignService.Create(campaign));

	[HttpPut("{id}")]
	public async Task<ActionResult<Campaign>> Update([FromBody] Campaign campaign)
		=> Ok(await _campaignService.Update(campaign));

	[HttpDelete("{id}")]
	public async Task<ActionResult<Campaign>> Delete([FromRoute] int id)
		=> Ok(await _campaignService.Delete(id));
}