namespace Solidarity.Application.Campaigns.Media;

public class SaveCampaignMedia
{
	private readonly IDatabase _database;
	public SaveCampaignMedia(IDatabase database) => _database = database;

	public async Task<CampaignMedia> Execute(CampaignMedia media)
	{
		media.Validate();
		return media.Id is not 0 && await _database.CampaignMedia.Get(media.Id) is not null
			? await _database.Update(media)
			: await _database.Create(media);
	}
}