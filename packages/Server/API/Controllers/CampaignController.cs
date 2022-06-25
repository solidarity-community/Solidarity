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

	[HttpGet("{id}/share"), AllowAnonymous]
	public async Task<ActionResult<decimal>> GetShare([FromRoute] int id) => Ok(await _campaignService.GetShare(id));

	[HttpGet("{id}/donation-data"), AllowAnonymous]
	public async Task<ActionResult<decimal>> GetDonationData([FromRoute] int id)
		=> Ok(await _campaignService.GetDonationData(id));

	[HttpPost("{id}/allocate"), AllowAnonymous]
	public async Task<ActionResult> Allocate([FromRoute] int id, [FromBody] Dictionary<string, string> destinationByPaymentMethodIdentifier)
	{
		await _campaignService.Allocate(id, destinationByPaymentMethodIdentifier);
		return Ok();
	}

	[HttpPost("{id}/declare-allocation-phase"), AllowAnonymous]
	public async Task<ActionResult> DeclareAllocationPhase([FromRoute] int id)
	{
		await _campaignService.DeclareAllocationPhase(id);
		return Ok();
	}

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