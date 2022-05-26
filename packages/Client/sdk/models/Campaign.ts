import { Model, Account, Validation, CampaignDonationChannel, CampaignMedia, CampaignExpenditure, model, CampaignMediaType } from 'sdk'
import { GeometryCollection } from 'geojson'

@model('Campaign')
export class Campaign extends Model {
	title?: string
	description?: string
	location?: GeometryCollection
	creator?: Account
	completion?: string
	donationChannels = new Array<CampaignDonationChannel>()
	media = new Array<CampaignMedia>()
	validationId?: number
	validation?: Validation
	expenditures = new Array<CampaignExpenditure>()

	get coverImageUri() {
		return this.media.find(m => m.type === CampaignMediaType.File)?.uri
	}

	get totalExpenditure() {
		return this.expenditures.map(e => e.totalPrice).reduce((a, acc) => a + acc, 0)
	}
}