import { Campaign, Model } from 'sdk'

export function getDonationChannelLogo(donationChannel: DonationChannel): string {
	return `/assets/payment-methods/${donationChannel.paymentMethodIdentifier.toLowerCase()}.svg`
}

export interface DonationChannel extends Model {
	campaign: Campaign
	paymentMethodIdentifier: string
}