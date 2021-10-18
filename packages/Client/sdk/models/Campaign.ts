import { Model, Account, Validation, DonationChannel, Point } from 'sdk'

export interface Campaign extends Model {
	title: string
	description: string
	location: Point
	creator?: Account
	completion?: string
	donationChannels?: Array<DonationChannel>
	validationId: number
	validation?: Validation
}