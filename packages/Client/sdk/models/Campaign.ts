import { Model, Account, Validation, DonationChannel } from 'sdk'
import { GeometryCollection } from 'geojson'

export interface Campaign extends Model {
	title?: string
	description?: string
	location?: GeometryCollection
	creator?: Account
	completion?: string
	donationChannels?: Array<DonationChannel>
	validationId?: number
	validation?: Validation
}