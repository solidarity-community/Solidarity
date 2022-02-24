import { Account, Campaign, Model } from 'sdk'

export interface DonationChannel extends Model {
	campaign: Campaign
	paymentMethodIdentifier: string
}