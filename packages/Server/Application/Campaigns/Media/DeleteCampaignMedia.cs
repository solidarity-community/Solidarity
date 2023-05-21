namespace Solidarity.Application.Campaigns.Media;

public class DeleteCampaignMedia
{
	private readonly IDatabase _database;
	private readonly FileService _fileService;
	public DeleteCampaignMedia(IDatabase database, FileService fileService) => (_database, _fileService) = (database, fileService);

	public async Task<CampaignMedia> Execute(int id)
	{
		var media = await _database.CampaignMedia.Get(id);

		if (media.Type == CampaignMediaType.File)
		{
			_fileService.Delete(media.Uri);
		}

		return await _database.Delete(media);
	}
}