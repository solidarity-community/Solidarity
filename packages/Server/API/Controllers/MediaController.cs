namespace Solidarity.Controllers;

[ApiController, Route("[controller]"), Authorize]
public class CampaignMediaController : ControllerBase
{
	private readonly CampaignMediaService _campaignMediaService;

	public CampaignMediaController(CampaignMediaService mediaService) => _campaignMediaService = mediaService;

	[HttpGet("{id}")]
	public ActionResult<CampaignMedia> Get([FromRoute] int id) => Ok(_campaignMediaService.Get(id));

	[HttpPost, HttpPut]
	public ActionResult<CampaignMedia> CreateOrUpdate([FromBody] CampaignMedia media) => Ok(_campaignMediaService.CreateOrUpdate(media));

	[HttpDelete]
	public ActionResult<CampaignMedia> Delete([FromRoute] int id) => Ok(_campaignMediaService.Delete(id));
}