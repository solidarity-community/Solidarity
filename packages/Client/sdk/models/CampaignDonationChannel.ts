import { Campaign, Model, model } from 'sdk'

@model('CampaignDonationChannel')
export class CampaignDonationChannel extends Model {
	campaign!: Campaign
	paymentMethodIdentifier!: string

	get logoSource() {
		return `/assets/payment-methods/${this.paymentMethodIdentifier.toLowerCase()}.svg`
	}
}