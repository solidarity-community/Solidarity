namespace Solidarity.Application.Campaigns.Media;

public sealed class SaveCampaignMedia(IDatabase database)
{
	public async Task<CampaignMedia> Execute(CampaignMedia media)
	{
		media.Validate();
		return media.Id is not 0 && await database.Get<CampaignMedia>(media.Id) is not null
			? await database.Update(media)
			: await database.Create(media);
	}
}