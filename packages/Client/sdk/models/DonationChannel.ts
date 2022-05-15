import { Campaign, Model, model } from 'sdk'

export function getDonationChannelLogo(donationChannel: DonationChannel): string {
	return `/assets/payment-methods/${donationChannel.paymentMethodIdentifier.toLowerCase()}.svg`
}

@model('DonationChannel')
export class DonationChannel extends Model {
	campaign!: Campaign
	paymentMethodIdentifier!: string
}