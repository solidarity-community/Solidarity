namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class CampaignController : ControllerBase
{
	private readonly CampaignService _campaignService;

	public CampaignController(CampaignService campainService) => _campaignService = campainService;

	[HttpGet, AllowAnonymous]
	public ActionResult<IEnumerable<Campaign>> GetAll() => Ok(_campaignService.GetAll());

	[HttpGet("{id}"), AllowAnonymous]
	public ActionResult<string> Get([FromRoute] int id) => Ok(_campaignService.Get(id));

	[HttpGet("{id}/balance"), AllowAnonymous]
	public ActionResult<decimal> GetBalance([FromRoute] int id) => Ok(_campaignService.GetBalance(id));

	[HttpPost]
	public ActionResult<Campaign> Create([FromBody, Bind(nameof(Campaign.CreatorId), nameof(Campaign.Description), nameof(Campaign.Title))] Campaign campaign)
		=> Ok(_campaignService.Create(campaign));

	[HttpPut("{id}")]
	public ActionResult<Campaign> Update([FromBody, Bind(nameof(Campaign.Id), nameof(Campaign.Description), nameof(Campaign.Title))] Campaign campaign)
		=> Ok(_campaignService.Update(campaign));

	[HttpDelete("{id}")]
	public ActionResult<Campaign> Delete([FromRoute] int id)
		=> Ok(_campaignService.Delete(id));
}