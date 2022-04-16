namespace Solidarity.Application.Services;

public class CampaignMediaService : CrudService<CampaignMedia>
{
	private readonly FileService _fileService;
	public CampaignMediaService(FileService fileService, IDatabase database, IPaymentMethodProvider paymentMethodProvider, ICurrentUserService currentUserService) : base(database, paymentMethodProvider, currentUserService)
	{
		_fileService = fileService;
	}

	public CampaignMedia CreateOrUpdate(CampaignMedia media)
	{
		return media.IsValid() == false
			? throw new InvalidMediaException()
			: media.Id is not 0 && Get(media.Id) is not null
				? Update(media)
				: Create(media);
	}

	public override CampaignMedia Delete(int id)
	{
		var media = Get(id);

		if (media.Type == CampaignMediaType.File)
		{
			_fileService.Delete(media.Uri);
		}

		return base.Delete(id);
	}
}