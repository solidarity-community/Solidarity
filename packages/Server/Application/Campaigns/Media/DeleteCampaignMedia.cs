namespace Solidarity.Application.Campaigns.Media;

public sealed class DeleteCampaignMedia(IDatabase database, FileService fileService)
{
	public async Task Execute(int id)
	{
		var media = await database.Get<CampaignMedia>(id);

		if (media.Type == CampaignMediaType.File)
		{
			fileService.Delete(media.Uri);
		}

		await database.Delete(media);
	}
}