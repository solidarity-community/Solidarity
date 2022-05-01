namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class CampaignMediaController : ControllerBase
{
	private readonly CampaignMediaService _campaignMediaService;

	public CampaignMediaController(CampaignMediaService mediaService) => _campaignMediaService = mediaService;

	[HttpGet("{id}")]
	public async Task<ActionResult<CampaignMedia>> Get([FromRoute] int id) => Ok(await _campaignMediaService.Get(id));

	[HttpPost, HttpPut]
	public async Task<ActionResult<CampaignMedia>> CreateOrUpdate([FromBody] CampaignMedia media) => Ok(await _campaignMediaService.CreateOrUpdate(media));

	[HttpDelete]
	public async Task<ActionResult<CampaignMedia>> Delete([FromRoute] int id) => Ok(await _campaignMediaService.Delete(id));
}