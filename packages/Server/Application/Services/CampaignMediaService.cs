namespace Solidarity.Application.Services;

public class CampaignMediaService : CrudService<CampaignMedia>
{
	private readonly FileService _fileService;
	public CampaignMediaService(FileService fileService, IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService)
	{
		_fileService = fileService;
	}

	public async Task<CampaignMedia> CreateOrUpdate(CampaignMedia media)
	{
		return media.IsValid() == false
			? throw new InvalidMediaException()
			: media.Id is not 0 && await Get(media.Id) is not null
				? await Update(media)
				: await Create(media);
	}

	public override async Task<CampaignMedia> Delete(int id)
	{
		var media = await Get(id);

		if (media.Type == CampaignMediaType.File)
		{
			_fileService.Delete(media.Uri);
		}

		return await base.Delete(id);
	}
}