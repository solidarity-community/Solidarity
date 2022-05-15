import { Campaign, Model, model } from 'sdk'

@model('DonationChannel')
export class DonationChannel extends Model {
	campaign!: Campaign
	paymentMethodIdentifier!: string

	get logoSource() {
		return `/assets/payment-methods/${this.paymentMethodIdentifier.toLowerCase()}.svg`
	}
}