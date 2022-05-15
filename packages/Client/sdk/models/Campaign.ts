import { Model, Account, Validation, DonationChannel, CampaignMedia, CampaignExpenditure, model, CampaignMediaType } from 'sdk'
import { GeometryCollection } from 'geojson'

@model('Campaign')
export class Campaign extends Model {
	title?: string
	description?: string
	location?: GeometryCollection
	creator?: Account
	completion?: string
	donationChannels?: Array<DonationChannel>
	media = new Array<CampaignMedia>()
	validationId?: number
	validation?: Validation
	expenditures = new Array<CampaignExpenditure>()

	get coverImageUri() { return this.media.find(m => m.type === CampaignMediaType.File)?.uri }
}