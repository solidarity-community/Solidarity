import { Model, Account, Validation, DonationChannel, CampaignMedia, CampaignExpenditure } from 'sdk'
import { GeometryCollection } from 'geojson'

export interface Campaign extends Model {
	title?: string
	description?: string
	location?: GeometryCollection
	creator?: Account
	completion?: string
	donationChannels?: Array<DonationChannel>
	media: Array<CampaignMedia>
	validationId?: number
	validation?: Validation
	expenditures: Array<CampaignExpenditure>
}